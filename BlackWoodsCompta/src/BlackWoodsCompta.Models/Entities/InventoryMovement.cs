using BlackWoodsCompta.Models.Enums;

namespace BlackWoodsCompta.Models.Entities;

public class InventoryMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public InventoryMovementType Type { get; set; }
    public string? Reason { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
