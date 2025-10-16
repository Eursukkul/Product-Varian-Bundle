namespace FlowAccount.Domain.Entities;

/// <summary>
/// คลังสินค้า
/// </summary>
public class Warehouse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
