namespace FlowAccount.Domain.Entities;

/// <summary>
/// จำนวนสต็อกสินค้าในแต่ละคลัง (รองรับ Product และ Variant)
/// Polymorphic relationship
/// </summary>
public class Stock
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    /// <summary>
    /// ประเภทของสินค้า: "Product" หรือ "Variant"
    /// </summary>
    public string ItemType { get; set; } = string.Empty; // "Product" or "Variant"

    /// <summary>
    /// ID ของสินค้า (ProductMaster.Id หรือ ProductVariant.Id)
    /// </summary>
    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Warehouse? Warehouse { get; set; }
}
