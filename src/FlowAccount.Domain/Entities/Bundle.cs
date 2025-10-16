namespace FlowAccount.Domain.Entities;

/// <summary>
/// ชุดสินค้า (Product Bundle)
/// </summary>
public class Bundle
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ICollection<BundleItem> BundleItems { get; set; } = new List<BundleItem>();
}
