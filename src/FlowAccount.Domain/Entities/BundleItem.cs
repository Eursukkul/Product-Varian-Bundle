namespace FlowAccount.Domain.Entities;

/// <summary>
/// รายการสินค้าใน Bundle (สามารถเป็น Product หรือ Variant ได้)
/// Polymorphic relationship
/// </summary>
public class BundleItem
{
    public int Id { get; set; }

    public int BundleId { get; set; }

    /// <summary>
    /// ประเภทของสินค้า: "Product" หรือ "Variant"
    /// </summary>
    public string ItemType { get; set; } = string.Empty; // "Product" or "Variant"

    /// <summary>
    /// ID ของสินค้า (ProductMaster.Id หรือ ProductVariant.Id)
    /// </summary>
    public int ItemId { get; set; }

    public int Quantity { get; set; }

    // Navigation Properties
    public virtual Bundle? Bundle { get; set; }
}
