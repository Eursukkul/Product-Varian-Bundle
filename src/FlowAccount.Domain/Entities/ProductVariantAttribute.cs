namespace FlowAccount.Domain.Entities;

/// <summary>
/// Junction Table: เชื่อม ProductVariant กับ VariantOptionValue
/// เช่น Variant "เสื้อยืดสีแดง S" = { สี: แดง, ขนาด: S }
/// </summary>
public class ProductVariantAttribute
{
    public int Id { get; set; }

    public int ProductVariantId { get; set; }

    public int VariantOptionValueId { get; set; }

    // Navigation Properties
    public virtual ProductVariant? ProductVariant { get; set; }

    public virtual VariantOptionValue? VariantOptionValue { get; set; }
}
