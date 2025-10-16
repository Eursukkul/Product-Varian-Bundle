namespace FlowAccount.Application.DTOs.Bundle;

/// <summary>
/// Request DTO for creating a Bundle
/// Example: Create a "Starter Pack" bundle with multiple products/variants
/// </summary>
public class CreateBundleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Bundle items (mix of Products and Variants)
    /// </summary>
    public List<CreateBundleItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request DTO for Bundle Item
/// </summary>
public class CreateBundleItemRequest
{
    /// <summary>
    /// Item type: "Product" or "Variant"
    /// </summary>
    public string ItemType { get; set; } = string.Empty;

    /// <summary>
    /// Product Master ID or Product Variant ID
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// Quantity of this item in the bundle
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// Response DTO for Create/Update Bundle operations
/// </summary>
public class BundleOperationResponse
{
    public BundleDto Bundle { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public List<string>? Warnings { get; set; }
}
