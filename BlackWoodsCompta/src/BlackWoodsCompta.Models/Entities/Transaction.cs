using BlackWoodsCompta.Models.Enums;

namespace BlackWoodsCompta.Models.Entities;

public class Transaction
{
    public int Id { get; set; }
    public TransactionType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
