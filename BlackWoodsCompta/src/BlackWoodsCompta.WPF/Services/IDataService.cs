using BlackWoodsCompta.Models.DTOs;
using BlackWoodsCompta.Models.Entities;

namespace BlackWoodsCompta.WPF.Services;

public interface IDataService
{
    // Auth
    Task<LoginResponse> LoginAsync(string username, string password);
    string GetConnectionInfo();
    
    // Transactions
    Task<List<Transaction>> GetTransactionsAsync(string? search = null, string? type = null, string? category = null);
    Task<Transaction?> CreateTransactionAsync(Transaction transaction);
    Task<bool> DeleteTransactionAsync(int id);
    
    // Dashboard
    Task<DashboardDto?> GetDashboardDataAsync();
    
    // Employees
    Task<List<Employee>> GetEmployeesAsync(string? search = null, string? position = null, bool? isActive = null);
    Task<Employee?> CreateEmployeeAsync(Employee employee);
    Task<bool> UpdateEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(int id);
    
    // Payrolls
    Task<List<Payroll>> GetPayrollsAsync(int? employeeId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<Payroll?> CreatePayrollAsync(Payroll payroll);
    Task<bool> DeletePayrollAsync(int id);
    
    // Inventory
    Task<List<InventoryItem>> GetInventoryAsync(string? search = null, string? category = null, bool? lowStock = null);
    Task<InventoryItem?> CreateInventoryItemAsync(InventoryItem item);
    Task<bool> UpdateInventoryItemAsync(InventoryItem item);
    Task<bool> DeleteInventoryItemAsync(int id);
    Task<List<InventoryMovement>> GetInventoryMovementsAsync(int? productId = null);
    Task<InventoryMovement?> CreateInventoryMovementAsync(InventoryMovement movement);
    
    // Invoices
    Task<List<Invoice>> GetInvoicesAsync(string? search = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<Invoice?> CreateInvoiceAsync(Invoice invoice);
    Task<bool> UpdateInvoiceAsync(Invoice invoice);
    Task<bool> DeleteInvoiceAsync(int id);
    Task<List<InvoiceItem>> GetInvoiceItemsAsync(int invoiceId);

    // Orders
    Task<List<Order>> GetOrdersAsync(string? search = null, string? status = null);
    Task<Order?> CreateOrderAsync(Order order);
    Task<bool> UpdateOrderAsync(Order order);
    Task<bool> DeleteOrderAsync(int id);
    Task<List<OrderItem>> GetOrderItemsAsync(int orderId);
    
    // Suppliers
    Task<List<Supplier>> GetSuppliersAsync(string? search = null);
    Task<Supplier?> CreateSupplierAsync(Supplier supplier);
    Task<bool> UpdateSupplierAsync(Supplier supplier);
    Task<bool> DeleteSupplierAsync(int id);
    
    // Purchase Prices
    Task<List<PurchasePrice>> GetPurchasePricesAsync(string? search = null, string? category = null, string? supplier = null);
    Task<PurchasePrice?> CreatePurchasePriceAsync(PurchasePrice price);
    Task<bool> UpdatePurchasePriceAsync(PurchasePrice price);
    Task<bool> DeletePurchasePriceAsync(int id);
    
    // Sale Prices
    Task<List<SalePrice>> GetSalePricesAsync(string? search = null, string? category = null);
    Task<SalePrice?> CreateSalePriceAsync(SalePrice price);
    Task<bool> UpdateSalePriceAsync(SalePrice price);
    Task<bool> DeleteSalePriceAsync(int id);
    
    // Employee Reimbursements
    Task<List<EmployeeReimbursement>> GetEmployeeReimbursementsAsync(string? search = null, string? status = null);
    Task<EmployeeReimbursement?> CreateEmployeeReimbursementAsync(EmployeeReimbursement reimbursement);
    Task<bool> UpdateEmployeeReimbursementAsync(EmployeeReimbursement reimbursement);
    Task<bool> DeleteEmployeeReimbursementAsync(int id);
}
