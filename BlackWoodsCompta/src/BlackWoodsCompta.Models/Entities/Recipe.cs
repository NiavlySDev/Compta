namespace BlackWoodsCompta.Models.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal CostPerUnit { get; set; }
    public int PreparationTime { get; set; } // en minutes
    public int Servings { get; set; } = 1;
    public string? Instructions { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
}

public class RecipeIngredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }

    // Navigation property
    public Recipe? Recipe { get; set; }
}