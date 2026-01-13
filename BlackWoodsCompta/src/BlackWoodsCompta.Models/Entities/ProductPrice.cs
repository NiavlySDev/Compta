namespace BlackWoodsCompta.Models.Entities;

public class ProductPrice
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal MarginPercent { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
}