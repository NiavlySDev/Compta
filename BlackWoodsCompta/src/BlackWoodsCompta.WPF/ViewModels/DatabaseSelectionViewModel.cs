using System.Windows.Input;
using BlackWoodsCompta.WPF.Helpers;

namespace BlackWoodsCompta.WPF.ViewModels;

public class DatabaseSelectionViewModel : ViewModelBase
{
    private string _apiUrl = "http://localhost:5000";
    private bool _useLocalDatabase;
    private bool _useApiDatabase = true;

    public string ApiUrl
    {
        get => _apiUrl;
        set => SetProperty(ref _apiUrl, value);
    }

    public bool UseLocalDatabase
    {
        get => _useLocalDatabase;
        set
        {
            if (SetProperty(ref _useLocalDatabase, value) && value)
            {
                UseApiDatabase = false;
            }
        }
    }

    public bool UseApiDatabase
    {
        get => _useApiDatabase;
        set
        {
            if (SetProperty(ref _useApiDatabase, value) && value)
            {
                UseLocalDatabase = false;
            }
        }
    }

    public ICommand ConfirmCommand { get; }
    public event EventHandler<(bool UseLocal, string ApiUrl)>? SelectionConfirmed;

    public DatabaseSelectionViewModel()
    {
        ConfirmCommand = new RelayCommand(ExecuteConfirm, CanExecuteConfirm);
    }

    private bool CanExecuteConfirm(object? parameter)
    {
        if (UseApiDatabase)
        {
            return !string.IsNullOrWhiteSpace(ApiUrl);
        }
        return UseLocalDatabase;
    }

    private void ExecuteConfirm(object? parameter)
    {
        SelectionConfirmed?.Invoke(this, (UseLocalDatabase, ApiUrl));
    }
}
