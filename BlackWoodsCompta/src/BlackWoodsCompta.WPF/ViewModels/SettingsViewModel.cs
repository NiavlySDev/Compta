using System.IO;
using System.Linq;
using System.Windows.Input;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using BlackWoodsCompta.WPF.Views;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private string _currentDatabaseMode = "Local";
    private string _applicationVersion = "1.0.0";
    private string _logPath = string.Empty;
    private string _databasePath = string.Empty;

    public string CurrentDatabaseMode
    {
        get => _currentDatabaseMode;
        set => SetProperty(ref _currentDatabaseMode, value);
    }

    public string ApplicationVersion
    {
        get => _applicationVersion;
        set => SetProperty(ref _applicationVersion, value);
    }

    public string LogPath
    {
        get => _logPath;
        set => SetProperty(ref _logPath, value);
    }

    public string DatabasePath
    {
        get => _databasePath;
        set => SetProperty(ref _databasePath, value);
    }

    public string CurrentUser => _authService.CurrentUser?.FullName ?? "Unknown";
    public string CurrentUserRole => _authService.CurrentUser?.Role.ToString() ?? "Unknown";

    public ICommand OpenLogsFolderCommand { get; }
    public ICommand OpenDatabaseFolderCommand { get; }
    public ICommand LogoutCommand { get; }

    public SettingsViewModel(IAuthService authService)
    {
        _authService = authService;

        // Determine current database mode from App settings
        _currentDatabaseMode = App.UseLocalDatabase ? "Base de données locale (SQLite)" : "Base de données distante (API)";

        // Set paths
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "BlackWoodsCompta");
        _logPath = Path.Combine(appFolder, "Logs");
        _databasePath = Path.Combine(appFolder, "Data");

        OpenLogsFolderCommand = new RelayCommand(_ => OpenLogsFolder());
        OpenDatabaseFolderCommand = new RelayCommand(_ => OpenDatabaseFolder());
        LogoutCommand = new RelayCommand(_ => Logout());

        Log.Information("Settings view loaded");
    }

    private void OpenLogsFolder()
    {
        try
        {
            if (Directory.Exists(_logPath))
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _logPath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(startInfo);
                Log.Information("Opened logs folder: {Path}", _logPath);
            }
            else
            {
                Log.Warning("Logs folder does not exist: {Path}", _logPath);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error opening logs folder");
        }
    }

    private void OpenDatabaseFolder()
    {
        try
        {
            if (Directory.Exists(_databasePath))
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _databasePath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(startInfo);
                Log.Information("Opened database folder: {Path}", _databasePath);
            }
            else
            {
                Log.Warning("Database folder does not exist: {Path}", _databasePath);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error opening database folder");
        }
    }

    private void Logout()
    {
        try
        {
            Log.Information("User logging out: {User}", CurrentUser);
            _authService.Logout();
            
            // Close main window first
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
            
            // Then reopen database selection window
            var dbWindow = new DatabaseSelectionWindow();
            if (dbWindow.ShowDialog() == true)
            {
                // User selected database, restart application flow
                System.Windows.Application.Current.Shutdown();
                System.Diagnostics.Process.Start(System.Environment.ProcessPath ?? "BlackWoodsCompta.exe");
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during logout");
        }
    }
}
