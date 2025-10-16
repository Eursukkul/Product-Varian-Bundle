namespace FlowAccount.Domain.Entities;

/// <summary>
/// ค่าของตัวเลือก Variant (เช่น "แดง", "น้ำเงิน" สำหรับสี)
/// </summary>
public class VariantOptionValue
{
    public int Id { get; set; }

    public int VariantOptionId { get; set; }

    public string Value { get; set; } = string.Empty; // e.g., "แดง", "S"

    public int DisplayOrder { get; set; }

    // Navigation Properties
    public virtual VariantOption? VariantOption { get; set; }

    public virtual ICollection<ProductVariantAttribute> ProductVariantAttributes { get; set; }
        = new List<ProductVariantAttribute>();
}
