using System.Windows.Input;
using BlackWoodsCompta.Models.DTOs;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private bool _isLoading;
    private decimal _totalRevenue;
    private decimal _totalExpenses;
    private decimal _netProfit;
    private int _transactionCount;
    private int _employeeCount;
    private int _lowStockItemsCount;
    private int _pendingInvoicesCount;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public decimal TotalRevenue
    {
        get => _totalRevenue;
        set => SetProperty(ref _totalRevenue, value);
    }

    public decimal TotalExpenses
    {
        get => _totalExpenses;
        set => SetProperty(ref _totalExpenses, value);
    }

    public decimal NetProfit
    {
        get => _netProfit;
        set => SetProperty(ref _netProfit, value);
    }

    public int TransactionCount
    {
        get => _transactionCount;
        set => SetProperty(ref _transactionCount, value);
    }

    public int EmployeeCount
    {
        get => _employeeCount;
        set => SetProperty(ref _employeeCount, value);
    }

    public int LowStockItemsCount
    {
        get => _lowStockItemsCount;
        set => SetProperty(ref _lowStockItemsCount, value);
    }

    public int PendingInvoicesCount
    {
        get => _pendingInvoicesCount;
        set => SetProperty(ref _pendingInvoicesCount, value);
    }

    public ICommand RefreshCommand { get; }

    public DashboardViewModel(IDataService dataService)
    {
        _dataService = dataService;
        RefreshCommand = new AsyncRelayCommand(LoadDashboardDataAsync);
        
        // Load data on initialization
        _ = LoadDashboardDataAsync(null);
    }

    private async Task LoadDashboardDataAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var data = await _dataService.GetDashboardDataAsync();

            if (data != null)
            {
                TotalRevenue = data.TotalRevenue;
                TotalExpenses = data.TotalExpenses;
                NetProfit = data.NetProfit;
                TransactionCount = data.TransactionCount;
                EmployeeCount = data.EmployeeCount;
                LowStockItemsCount = data.LowStockItemsCount;
                PendingInvoicesCount = data.PendingInvoicesCount;

                Log.Information("Dashboard data loaded successfully");
            }
            else
            {
                Log.Warning("Failed to load dashboard data");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading dashboard data");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
