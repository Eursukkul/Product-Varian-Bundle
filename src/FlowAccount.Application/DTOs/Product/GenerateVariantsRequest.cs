namespace FlowAccount.Application.DTOs.Product;

/// <summary>
/// Request DTO for generating multiple Product Variants
/// This supports BATCH OPERATIONS - creating up to 250 variants at once
/// </summary>
public class GenerateVariantsRequest
{
    public int ProductMasterId { get; set; }

    /// <summary>
    /// Selected option values for generating variants
    /// Key: VariantOptionId, Value: List of VariantOptionValueIds
    /// Example: { 1: [1, 2], 2: [3, 4, 5] } 
    /// → Generates 2 × 3 = 6 variants (Color × Size)
    /// </summary>
    public Dictionary<int, List<int>> SelectedOptions { get; set; } = new();

    /// <summary>
    /// Price calculation strategy
    /// </summary>
    public PriceStrategy PriceStrategy { get; set; } = PriceStrategy.Fixed;

    /// <summary>
    /// Base price for variants (used with Fixed strategy)
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// Base cost for variants
    /// </summary>
    public decimal BaseCost { get; set; }

    /// <summary>
    /// SKU generation pattern
    /// Example: "TSHIRT-{Color}-{Size}" → "TSHIRT-RED-M"
    /// </summary>
    public string? SkuPattern { get; set; }
}

/// <summary>
/// Price calculation strategies for variants
/// </summary>
public enum PriceStrategy
{
    Fixed,          // All variants same price
    SizeAdjusted,   // Adjust price based on size (S: +0, M: +10, L: +20)
    ColorAdjusted   // Adjust price based on color
}

/// <summary>
/// Response DTO for batch variant generation
/// </summary>
public class GenerateVariantsResponse
{
    public int ProductMasterId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalVariantsGenerated { get; set; }
    public List<ProductVariantDto> Variants { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
    public string Message { get; set; } = string.Empty;
}
