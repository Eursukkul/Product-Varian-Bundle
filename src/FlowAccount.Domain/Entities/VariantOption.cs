namespace FlowAccount.Domain.Entities;

/// <summary>
/// ตัวเลือกของ Variant (เช่น สี, ขนาด)
/// </summary>
public class VariantOption
{
    public int Id { get; set; }

    public int ProductMasterId { get; set; }

    public string Name { get; set; } = string.Empty; // e.g., "สี", "ขนาด"

    public int DisplayOrder { get; set; }

    // Navigation Properties
    public virtual ProductMaster? ProductMaster { get; set; }

    public virtual ICollection<VariantOptionValue> Values { get; set; } = new List<VariantOptionValue>();
}
