namespace BlackWoodsCompta.Models.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Matière première, Plat préparé
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime? ExpiryDate { get; set; } // Pour plats préparés
}
