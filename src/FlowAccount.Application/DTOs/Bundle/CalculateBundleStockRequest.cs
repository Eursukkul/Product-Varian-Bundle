namespace FlowAccount.Application.DTOs.Bundle;

/// <summary>
/// Request DTO for calculating Bundle stock
/// This handles the complex STOCK LOGIC requirement
/// </summary>
public class CalculateBundleStockRequest
{
    public int BundleId { get; set; }
    public int WarehouseId { get; set; }
}

/// <summary>
/// Response DTO for Bundle stock calculation
/// Shows detailed breakdown of how stock is calculated
/// </summary>
public class BundleStockCalculationResponse
{
    public int BundleId { get; set; }
    public string BundleName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of bundles that can be made
    /// This is the MINIMUM of all item's possible bundles
    /// </summary>
    public int MaxAvailableBundles { get; set; }

    /// <summary>
    /// Detailed breakdown for each item in the bundle
    /// </summary>
    public List<BundleItemStockInfo> ItemsStockBreakdown { get; set; } = new();

    /// <summary>
    /// Calculation explanation
    /// </summary>
    public string Explanation { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when calculation was performed
    /// </summary>
    public DateTime CalculatedAt { get; set; }
}

/// <summary>
/// Stock information for each item in a bundle
/// </summary>
public class BundleItemStockInfo
{
    public string ItemType { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? ItemSKU { get; set; }

    /// <summary>
    /// Quantity required per bundle
    /// </summary>
    public int RequiredQuantity { get; set; }

    /// <summary>
    /// Available quantity in warehouse
    /// </summary>
    public int AvailableQuantity { get; set; }

    /// <summary>
    /// How many bundles can be made with this item
    /// Formula: AvailableQuantity / RequiredQuantity
    /// </summary>
    public int PossibleBundles { get; set; }

    /// <summary>
    /// Is this item the bottleneck?
    /// </summary>
    public bool IsBottleneck { get; set; }
}
