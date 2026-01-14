using System.Windows.Input;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using BlackWoodsCompta.WPF.Views;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private object? _currentView;
    private string _currentUserName = string.Empty;
    private string _currentUserRole = string.Empty;

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public string CurrentUserName
    {
        get => _currentUserName;
        set => SetProperty(ref _currentUserName, value);
    }

    public string CurrentUserRole
    {
        get => _currentUserRole;
        set => SetProperty(ref _currentUserRole, value);
    }

    public ICommand NavigateToDashboardCommand { get; }
    public ICommand NavigateToTransactionsCommand { get; }
    public ICommand NavigateToEmployeesCommand { get; }
    public ICommand NavigateToPayrollsCommand { get; }
    public ICommand NavigateToInventoryCommand { get; }
    public ICommand NavigateToOrdersCommand { get; }
    public ICommand NavigateToInvoicesCommand { get; }
    public ICommand NavigateToCashRegisterCommand { get; }
    public ICommand NavigateToSuppliersCommand { get; }
    public ICommand NavigateToPurchasePricesCommand { get; }
    public ICommand NavigateToSalePricesCommand { get; }
    public ICommand NavigateToReimbursementsCommand { get; }
    public ICommand NavigateToReportsCommand { get; }
    public ICommand NavigateToSettingsCommand { get; }
    public ICommand LogoutCommand { get; }

    public MainViewModel(IAuthService authService)
    {
        _authService = authService;
        
        if (_authService.CurrentUser != null)
        {
            CurrentUserName = _authService.CurrentUser.FullName;
            CurrentUserRole = _authService.CurrentUser.Role.ToString();
        }

        NavigateToDashboardCommand = new RelayCommand(_ => NavigateToDashboard());
        NavigateToTransactionsCommand = new RelayCommand(_ => NavigateToTransactions());
        NavigateToEmployeesCommand = new RelayCommand(_ => NavigateToEmployees());
        NavigateToPayrollsCommand = new RelayCommand(_ => NavigateToPayrolls());
        NavigateToInventoryCommand = new RelayCommand(_ => NavigateToInventory());
        NavigateToOrdersCommand = new RelayCommand(_ => NavigateToOrders());
        NavigateToInvoicesCommand = new RelayCommand(_ => NavigateToInvoices());
        NavigateToCashRegisterCommand = new RelayCommand(_ => NavigateToCashRegister());
        NavigateToSuppliersCommand = new RelayCommand(_ => NavigateToSuppliers());
        NavigateToPurchasePricesCommand = new RelayCommand(_ => NavigateToPurchasePrices());
        NavigateToSalePricesCommand = new RelayCommand(_ => NavigateToSalePrices());
        NavigateToReimbursementsCommand = new RelayCommand(_ => NavigateToReimbursements());
        NavigateToReportsCommand = new RelayCommand(_ => NavigateToReports());
        NavigateToSettingsCommand = new RelayCommand(_ => NavigateToSettings());
        LogoutCommand = new RelayCommand(_ => Logout());

        // Load dashboard by default
        NavigateToDashboard();
    }

    private void NavigateToDashboard()
    {
        var dashboardView = App.ServiceProvider?.GetService(typeof(DashboardView)) as DashboardView;
        if (dashboardView != null)
        {
            CurrentView = dashboardView;
        }
    }

    private void NavigateToTransactions()
    {
        var transactionsView = App.ServiceProvider?.GetService(typeof(TransactionsView)) as TransactionsView;
        if (transactionsView != null)
        {
            CurrentView = transactionsView;
        }
    }

    private void NavigateToEmployees()
    {
        var view = App.ServiceProvider?.GetService(typeof(EmployeesView)) as EmployeesView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToPayrolls()
    {
        var view = App.ServiceProvider?.GetService(typeof(PayrollsView)) as PayrollsView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToInventory()
    {
        var view = App.ServiceProvider?.GetService(typeof(InventoryView)) as InventoryView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToOrders()
    {
        var view = App.ServiceProvider?.GetService(typeof(OrdersView)) as OrdersView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToInvoices()
    {
        var view = App.ServiceProvider?.GetService(typeof(InvoicesView)) as InvoicesView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToCashRegister()
    {
        var view = App.ServiceProvider?.GetService(typeof(CashRegisterView)) as CashRegisterView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToReports()
    {
        var view = App.ServiceProvider?.GetService(typeof(ReportsView)) as ReportsView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToSettings()
    {
        var view = App.ServiceProvider?.GetService(typeof(SettingsView)) as SettingsView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToSuppliers()
    {
        var view = App.ServiceProvider?.GetService(typeof(SuppliersView)) as SuppliersView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToPurchasePrices()
    {
        var view = App.ServiceProvider?.GetService(typeof(PurchasePricesView)) as PurchasePricesView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToSalePrices()
    {
        var view = App.ServiceProvider?.GetService(typeof(SalePricesView)) as SalePricesView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void NavigateToReimbursements()
    {
        var view = App.ServiceProvider?.GetService(typeof(ReimbursementsView)) as ReimbursementsView;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    private void Logout()
    {
        try
        {
            Log.Information("User logging out from MainViewModel");
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
            Log.Error(ex, "Error during logout from MainViewModel");
        }
    }
}
