using BlackWoodsCompta.Models.Enums;

namespace BlackWoodsCompta.Models.Entities;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? ClientPhone { get; set; }
    public string? ClientEmail { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public int CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public List<InvoiceItem> Items { get; set; } = new();
}
