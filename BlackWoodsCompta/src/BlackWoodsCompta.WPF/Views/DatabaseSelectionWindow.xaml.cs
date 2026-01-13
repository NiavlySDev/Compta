using System.Windows;
using System.Windows.Input;
using BlackWoodsCompta.WPF.ViewModels;

namespace BlackWoodsCompta.WPF.Views;

public partial class DatabaseSelectionWindow : Window
{
    private readonly DatabaseSelectionViewModel _viewModel;

    public bool UseLocalDatabase { get; private set; }
    public string ApiUrl { get; private set; } = string.Empty;

    public DatabaseSelectionWindow()
    {
        InitializeComponent();
        _viewModel = new DatabaseSelectionViewModel();
        DataContext = _viewModel;
        
        _viewModel.SelectionConfirmed += OnSelectionConfirmed;
    }

    private void OnSelectionConfirmed(object? sender, (bool UseLocal, string ApiUrl) selection)
    {
        UseLocalDatabase = selection.UseLocal;
        ApiUrl = selection.ApiUrl;
        
        // Only set DialogResult if opened as dialog
        if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
        {
            DialogResult = true;
        }
        
        Close();
    }

    private void LocalDatabase_Click(object sender, MouseButtonEventArgs e)
    {
        _viewModel.UseLocalDatabase = true;
    }

    private void ApiDatabase_Click(object sender, MouseButtonEventArgs e)
    {
        _viewModel.UseApiDatabase = true;
    }
}
