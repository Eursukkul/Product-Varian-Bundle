namespace FlowAccount.Application.DTOs.Bundle;

/// <summary>
/// Bundle DTO for responses
/// </summary>
public class BundleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Bundle composition
    public List<BundleItemDto> Items { get; set; } = new();

    // Stock information (calculated)
    public int? AvailableStock { get; set; }
    public string? StockMessage { get; set; }
}

/// <summary>
/// Bundle Item DTO
/// </summary>
public class BundleItemDto
{
    public int Id { get; set; }
    public string ItemType { get; set; } = string.Empty; // "Product" or "Variant"
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? ItemSKU { get; set; } // Only for Variant
    public int Quantity { get; set; }

    // Stock info for this item
    public int? AvailableStock { get; set; }
    public int? PossibleBundles { get; set; } // How many bundles can be made from this item
}
