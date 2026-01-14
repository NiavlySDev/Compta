using BlackWoodsCompta.Models.DTOs;
using BlackWoodsCompta.Models.Entities;
using Serilog;

namespace BlackWoodsCompta.WPF.Services;

public class ApiDataService : IDataService
{
    private readonly IApiService _apiService;

    public ApiDataService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        var request = new LoginRequest { Username = username, Password = password };
        var response = await _apiService.PostAsync<LoginResponse>("/api/auth/login", request);
        return response ?? new LoginResponse { Success = false, Message = "Erreur de connexion" };
    }

    public async Task<List<Transaction>> GetTransactionsAsync(string? search = null, string? type = null, string? category = null)
    {
        var endpoint = "/api/transactions";
        if (!string.IsNullOrWhiteSpace(search))
        {
            endpoint += $"?search={search}";
        }

        var response = await _apiService.GetAsync<ApiResponse<List<Transaction>>>(endpoint);
        return response?.Data ?? new List<Transaction>();
    }

    public async Task<Transaction?> CreateTransactionAsync(Transaction transaction)
    {
        var response = await _apiService.PostAsync<ApiResponse<Transaction>>("/api/transactions", transaction);
        return response?.Data;
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        return await _apiService.DeleteAsync($"/api/transactions/{id}");
    }

    public async Task<DashboardDto?> GetDashboardDataAsync()
    {
        var response = await _apiService.GetAsync<ApiResponse<DashboardDto>>("/api/reports/dashboard");
        return response?.Data;
    }

    public async Task<List<Employee>> GetEmployeesAsync(string? search = null, string? position = null, bool? isActive = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrWhiteSpace(position)) queryParams.Add($"position={Uri.EscapeDataString(position)}");
        if (isActive.HasValue) queryParams.Add($"isActive={isActive.Value}");

        var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        var response = await _apiService.GetAsync<ApiResponse<List<Employee>>>($"/api/employees{query}");
        return response?.Data ?? new List<Employee>();
    }

    public async Task<Employee?> CreateEmployeeAsync(Employee employee)
    {
        var response = await _apiService.PostAsync<ApiResponse<Employee>>("/api/employees", employee);
        return response?.Data;
    }

    public async Task<bool> UpdateEmployeeAsync(Employee employee)
    {
        var response = await _apiService.PutAsync<ApiResponse<Employee>>($"/api/employees/{employee.Id}", employee);
        return response?.Success ?? false;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        return await _apiService.DeleteAsync($"/api/employees/{id}");
    }

    // Payrolls
    public async Task<List<Payroll>> GetPayrollsAsync(int? employeeId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = new List<string>();
        if (employeeId.HasValue) queryParams.Add($"employeeId={employeeId.Value}");
        if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

        var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        var response = await _apiService.GetAsync<ApiResponse<List<Payroll>>>($"/api/payrolls{query}");
        return response?.Data ?? new List<Payroll>();
    }

    public async Task<Payroll?> CreatePayrollAsync(Payroll payroll)
    {
        var response = await _apiService.PostAsync<ApiResponse<Payroll>>("/api/payrolls", payroll);
        return response?.Data;
    }

    public async Task<bool> DeletePayrollAsync(int id)
    {
        return await _apiService.DeleteAsync($"/api/payrolls/{id}");
    }

    // Inventory
    public async Task<List<InventoryItem>> GetInventoryAsync(string? search = null, string? category = null, bool? lowStock = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrWhiteSpace(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
        if (lowStock.HasValue) queryParams.Add($"lowStock={lowStock.Value}");

        var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        var response = await _apiService.GetAsync<ApiResponse<List<InventoryItem>>>($"/api/inventory{query}");
        return response?.Data ?? new List<InventoryItem>();
    }

    public async Task<InventoryItem?> CreateInventoryItemAsync(InventoryItem item)
    {
        var response = await _apiService.PostAsync<ApiResponse<InventoryItem>>("/api/inventory", item);
        return response?.Data;
    }

    public async Task<bool> UpdateInventoryItemAsync(InventoryItem item)
    {
        var response = await _apiService.PutAsync<ApiResponse<InventoryItem>>($"/api/inventory/{item.Id}", item);
        return response?.Success ?? false;
    }

    public async Task<bool> DeleteInventoryItemAsync(int id)
    {
        return await _apiService.DeleteAsync($"/api/inventory/{id}");
    }

    public async Task<List<InventoryMovement>> GetInventoryMovementsAsync(int? productId = null)
    {
        var query = productId.HasValue ? $"?productId={productId.Value}" : "";
        var response = await _apiService.GetAsync<ApiResponse<List<InventoryMovement>>>($"/api/inventory/movements{query}");
        return response?.Data ?? new List<InventoryMovement>();
    }

    public async Task<InventoryMovement?> CreateInventoryMovementAsync(InventoryMovement movement)
    {
        var response = await _apiService.PostAsync<ApiResponse<InventoryMovement>>("/api/inventory/movements", movement);
        return response?.Data;
    }

    // Invoices
    public async Task<List<Invoice>> GetInvoicesAsync(string? search = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrWhiteSpace(status)) queryParams.Add($"status={Uri.EscapeDataString(status)}");
        if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

        var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        var response = await _apiService.GetAsync<ApiResponse<List<Invoice>>>($"/api/invoices{query}");
        return response?.Data ?? new List<Invoice>();
    }

    public async Task<Invoice?> CreateInvoiceAsync(Invoice invoice)
    {
        var response = await _apiService.PostAsync<ApiResponse<Invoice>>("/api/invoices", invoice);
        return response?.Data;
    }

    public async Task<bool> UpdateInvoiceAsync(Invoice invoice)
    {
        var response = await _apiService.PutAsync<ApiResponse<Invoice>>($"/api/invoices/{invoice.Id}", invoice);
        return response?.Success ?? false;
    }

    public async Task<bool> DeleteInvoiceAsync(int id)
    {
        return await _apiService.DeleteAsync($"/api/invoices/{id}");
    }

    public async Task<List<InvoiceItem>> GetInvoiceItemsAsync(int invoiceId)
    {
        var response = await _apiService.GetAsync<ApiResponse<List<InvoiceItem>>>($"/api/invoices/{invoiceId}/items");
        return response?.Data ?? new List<InvoiceItem>();
    }

    // Inventory operations - déjà existante
    // public async Task<List<InventoryItem>> GetInventoryAsync(string? search = null, string? category = null, bool? lowStock = null)
    // {
    //     var queryParams = new List<string>();
    //     if (!string.IsNullOrWhiteSpace(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
    //     if (!string.IsNullOrWhiteSpace(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
    //     if (lowStock.HasValue) queryParams.Add($"lowStock={lowStock.Value}");

    //     var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
    //     var response = await _apiService.GetAsync<ApiResponse<List<InventoryItem>>>($"/api/inventory{query}");
    //     return response?.Data ?? new List<InventoryItem>();
    // }

    // Orders operations
    public async Task<List<Order>> GetOrdersAsync(string? search = null, string? status = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrWhiteSpace(status)) queryParams.Add($"status={Uri.EscapeDataString(status)}");

        var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        var response = await _apiService.GetAsync<ApiResponse<List<Order>>>($"/api/orders{query}");
        return response?.Data ?? new List<Order>();
    }

    public async Task<Order?> CreateOrderAsync(Order order)
    {
        var response = await _apiService.PostAsync<ApiResponse<Order>>("/api/orders", order);
        return response?.Data;
    }

    public async Task<bool> UpdateOrderAsync(Order order)
    {
        var response = await _apiService.PutAsync<ApiResponse<Order>>($"/api/orders/{order.Id}", order);
        return response?.Success ?? false;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        return await _apiService.DeleteAsync($"/api/orders/{id}");
    }

    public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
    {
        var response = await _apiService.GetAsync<ApiResponse<List<OrderItem>>>($"/api/orders/{orderId}/items");
        return response?.Data ?? new List<OrderItem>();
    }

    public string GetConnectionInfo()
    {
        return $"API: {_apiService.BaseUrl}";
    }

    // Missing methods delegation
    public async Task<bool> UpdateTransactionAsync(Transaction transaction)
    {
        var response = await _apiService.PutAsync<ApiResponse<Transaction>>($"/api/transactions/{transaction.Id}", transaction);
        return response?.Success ?? false;
    }

    // Supplier methods
    public async Task<List<Supplier>> GetSuppliersAsync(string? search = null)
    {
        var url = "/api/suppliers";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"?search={Uri.EscapeDataString(search)}";
            
        var response = await _apiService.GetAsync<ApiResponse<List<Supplier>>>(url);
        return response?.Data ?? new List<Supplier>();
    }

    public async Task<Supplier?> CreateSupplierAsync(Supplier supplier)
    {
        var response = await _apiService.PostAsync<ApiResponse<Supplier>>("/api/suppliers", supplier);
        return response?.Data;
    }

    public async Task<bool> UpdateSupplierAsync(Supplier supplier)
    {
        var response = await _apiService.PutAsync<ApiResponse<Supplier>>($"/api/suppliers/{supplier.Id}", supplier);
        return response?.Success ?? false;
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        return await _apiService.DeleteAsync($"/api/suppliers/{id}");
    }
}
