namespace BlackWoodsCompta.Models.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string Status { get; set; } = "En attente"; // En attente, Livrée, Annulée
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public int UserId { get; set; }
    public int? TransactionId { get; set; } // Lié à la dépense créée
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public List<OrderItem> Items { get; set; } = new();
}
