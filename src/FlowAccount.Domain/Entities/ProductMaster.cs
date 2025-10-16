namespace FlowAccount.Domain.Entities;

/// <summary>
/// สินค้าหลัก (Product Master) - ใช้เป็นตัวกลางสำหรับสินค้าที่มี Variants
/// </summary>
public class ProductMaster
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Category? Category { get; set; }

    public virtual ICollection<VariantOption> VariantOptions { get; set; } = new List<VariantOption>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
