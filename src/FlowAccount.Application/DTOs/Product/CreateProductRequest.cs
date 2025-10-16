namespace FlowAccount.Application.DTOs.Product;

/// <summary>
/// Request DTO for creating a Product Master with Variant Options
/// Example: Create a T-Shirt product with Color and Size options
/// </summary>
public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    // Variant Options (e.g., Color, Size)
    public List<CreateVariantOptionRequest> VariantOptions { get; set; } = new();
}

/// <summary>
/// Request DTO for creating a Variant Option
/// </summary>
public class CreateVariantOptionRequest
{
    public string Name { get; set; } = string.Empty; // e.g., "Color"
    public int DisplayOrder { get; set; }
    public List<string> Values { get; set; } = new(); // e.g., ["Red", "Blue", "Green"]
}
