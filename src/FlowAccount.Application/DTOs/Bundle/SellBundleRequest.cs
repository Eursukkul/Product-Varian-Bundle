namespace FlowAccount.Application.DTOs.Bundle;

/// <summary>
/// Request DTO for selling a Bundle
/// This handles TRANSACTION MANAGEMENT and stock deduction
/// </summary>
public class SellBundleRequest
{
    public int BundleId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }

    /// <summary>
    /// Optional: Force sell even if stock is low
    /// </summary>
    public bool AllowBackorder { get; set; } = false;
}

/// <summary>
/// Response DTO for Bundle sale transaction
/// </summary>
public class SellBundleResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public int BundleId { get; set; }
    public string BundleName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Stock deduction details for each item
    /// </summary>
    public List<StockDeductionInfo> StockDeductions { get; set; } = new();

    /// <summary>
    /// Remaining stock after sale
    /// </summary>
    public int RemainingBundleStock { get; set; }

    /// <summary>
    /// Transaction ID (for reference)
    /// </summary>
    public string? TransactionId { get; set; }

    public DateTime TransactionDate { get; set; }
}

/// <summary>
/// Stock deduction information for an item
/// </summary>
public class StockDeductionInfo
{
    public string ItemType { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int QuantityDeducted { get; set; }
    public int StockBefore { get; set; }
    public int StockAfter { get; set; }
}
