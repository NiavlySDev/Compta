namespace BlackWoodsCompta.Models.Entities;

public class InventoryItem
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Matière première, Plat préparé
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public decimal MinQuantity { get; set; }
    public string? Supplier { get; set; }
    public DateTime? ExpiryDate { get; set; } // Null pour matières premières, 1 semaine pour plats préparés
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public bool IsLowStock => Quantity <= MinQuantity;
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now;
    public bool IsExpiringSoon => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now.AddDays(2);
}
