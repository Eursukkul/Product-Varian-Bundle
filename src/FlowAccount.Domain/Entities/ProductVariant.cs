namespace FlowAccount.Domain.Entities;

/// <summary>
/// สินค้า Variant (เช่น เสื้อยืดสีแดง ไซส์ S)
/// </summary>
public class ProductVariant
{
    public int Id { get; set; }

    public int ProductMasterId { get; set; }

    public string SKU { get; set; } = string.Empty; // รหัสสินค้า (UNIQUE)

    public decimal Price { get; set; }

    public decimal Cost { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ProductMaster? ProductMaster { get; set; }

    public virtual ICollection<ProductVariantAttribute> Attributes { get; set; }
        = new List<ProductVariantAttribute>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
