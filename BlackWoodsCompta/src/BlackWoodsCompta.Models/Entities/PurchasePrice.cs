namespace BlackWoodsCompta.Models.Entities;

public class PurchasePrice
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public DateTime? LastUpdated { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}