namespace BlackWoodsCompta.Models.DTOs;

public class DashboardDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public int TransactionCount { get; set; }
    public int EmployeeCount { get; set; }
    public int LowStockItemsCount { get; set; }
    public int PendingInvoicesCount { get; set; }
    public List<ChartDataPoint> RevenueChart { get; set; } = new();
    public List<ChartDataPoint> ExpensesChart { get; set; } = new();
    public List<CategoryBreakdown> ExpensesByCategory { get; set; } = new();
}

public class ChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class CategoryBreakdown
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}
