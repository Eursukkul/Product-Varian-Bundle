namespace FlowAccount.Domain.Entities;

/// <summary>
/// หมวดหมู่สินค้า
/// </summary>
public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ICollection<ProductMaster> Products { get; set; } = new List<ProductMaster>();
}
