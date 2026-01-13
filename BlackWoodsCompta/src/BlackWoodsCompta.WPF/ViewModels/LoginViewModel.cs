using System.Windows.Input;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoginCommand { get; }
    public event EventHandler<bool>? LoginCompleted;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        LoginCommand = new AsyncRelayCommand(ExecuteLoginAsync, CanExecuteLogin);
        
        // Force command re-evaluation when properties change
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Username) || e.PropertyName == nameof(Password) || e.PropertyName == nameof(IsLoading))
            {
                (LoginCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        };
    }

    private bool CanExecuteLogin(object? parameter)
    {
        var canExecute = !string.IsNullOrWhiteSpace(Username) && 
               !string.IsNullOrWhiteSpace(Password) && 
               !IsLoading;
        Log.Debug($"CanExecuteLogin: {canExecute} (Username: '{Username}', Password: '{Password?.Length} chars', IsLoading: {IsLoading})");
        return canExecute;
    }

    private async Task ExecuteLoginAsync(object? parameter)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            Log.Information($"Attempting login for user: {Username}");
            var response = await _authService.LoginAsync(Username, Password);
            Log.Information($"Login response: Success={response.Success}, Message={response.Message}");

            if (response.Success)
            {
                Log.Information($"Login successful for user: {Username}");
                LoginCompleted?.Invoke(this, true);
            }
            else
            {
                ErrorMessage = response.Message ?? "Erreur de connexion";
                Log.Warning($"Login failed for user: {Username}, Message: {response.Message}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
            Log.Error(ex, "Login error in ViewModel");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
