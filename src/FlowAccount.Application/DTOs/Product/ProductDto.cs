namespace FlowAccount.Application.DTOs.Product;

/// <summary>
/// Product Master DTO for responses
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public List<VariantOptionDto>? VariantOptions { get; set; }
    public List<ProductVariantDto>? ProductVariants { get; set; }
}

/// <summary>
/// Variant Option DTO
/// </summary>
public class VariantOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public List<VariantOptionValueDto> Values { get; set; } = new();
}

/// <summary>
/// Variant Option Value DTO
/// </summary>
public class VariantOptionValueDto
{
    public int Id { get; set; }
    public int VariantOptionId { get; set; }
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Product Variant DTO
/// </summary>
public class ProductVariantDto
{
    public int Id { get; set; }
    public int ProductMasterId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Attributes (e.g., Color: Red, Size: M)
    public List<VariantAttributeDto> Attributes { get; set; } = new();
}

/// <summary>
/// Variant Attribute DTO (for displaying variant properties)
/// </summary>
public class VariantAttributeDto
{
    public string OptionName { get; set; } = string.Empty; // e.g., "Color"
    public string OptionValue { get; set; } = string.Empty; // e.g., "Red"
    public int VariantOptionValueId { get; set; }
}
