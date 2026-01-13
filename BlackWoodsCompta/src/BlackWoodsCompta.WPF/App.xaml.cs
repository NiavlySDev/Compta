using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using BlackWoodsCompta.WPF.Services;
using BlackWoodsCompta.WPF.ViewModels;
using BlackWoodsCompta.WPF.Views;
using Serilog;
using System.IO;

namespace BlackWoodsCompta.WPF;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public static bool UseLocalDatabase { get; private set; }

    public App()
    {
        // Handle unhandled exceptions
        this.DispatcherUnhandledException += (s, e) =>
        {
            MessageBox.Show($"Erreur: {e.Exception.Message}\n\nStack trace:\n{e.Exception.StackTrace}", 
                "Erreur d'application", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        };
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // Configure Serilog
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BlackWoodsCompta",
                "Logs",
                "app.log"
            );

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Application starting...");

            // Show database selection window
            var dbSelectionWindow = new DatabaseSelectionWindow();
            if (dbSelectionWindow.ShowDialog() == true)
            {
                // Store database mode in static property
                UseLocalDatabase = dbSelectionWindow.UseLocalDatabase;
                
                // Configure services based on selection
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection, dbSelectionWindow.UseLocalDatabase, dbSelectionWindow.ApiUrl);
                ServiceProvider = serviceCollection.BuildServiceProvider();

                Log.Information($"Database mode: {(dbSelectionWindow.UseLocalDatabase ? "Local" : "API")}");

                // Show login window
                var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
            }
            else
            {
                // User cancelled - exit application
                Shutdown();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur au démarrage: {ex.Message}\n\nStack trace:\n{ex.StackTrace}", 
                "Erreur critique", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    private void ConfigureServices(IServiceCollection services, bool useLocalDb, string apiUrl)
    {
        // Services
        if (useLocalDb)
        {
            services.AddSingleton<IDataService, LocalDataService>();
        }
        else
        {
            var apiService = new ApiService();
            apiService.UpdateBaseUrl(apiUrl);
            services.AddSingleton<IApiService>(apiService);
            services.AddSingleton<IDataService, ApiDataService>();
        }
        
        services.AddSingleton<IAuthService, AuthService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<TransactionsViewModel>();
        services.AddTransient<EmployeesViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<OrdersViewModel>();
        services.AddTransient<SuppliersViewModel>();
        services.AddTransient<ReimbursementsViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Views
        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<DashboardView>();
        services.AddTransient<TransactionsView>();
        services.AddTransient<EmployeesView>();
        services.AddTransient<PayrollsView>();
        services.AddTransient<InventoryView>();
        services.AddTransient<OrdersView>();
        services.AddTransient<SuppliersView>();
        services.AddTransient<ReimbursementsView>();
        services.AddTransient<InvoicesView>();
        services.AddTransient<CashRegisterView>();
        services.AddTransient<ReportsView>();
        services.AddTransient<SettingsView>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Application closing...");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
