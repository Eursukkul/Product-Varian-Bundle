using FlowAccount.Domain.Entities;

namespace FlowAccount.Domain.Interfaces;

/// <summary>
/// Variant Repository Interface
/// </summary>
public interface IVariantRepository : IRepository<ProductVariant>
{
    Task<bool> ExistsBySKUAsync(string sku, int? excludeVariantId = null);

    Task<IEnumerable<ProductVariant>> GetVariantsByProductIdAsync(int productId);

    Task<ProductVariant?> GetVariantWithAttributesAsync(int id);

    Task<IEnumerable<ProductVariant>> GetVariantsBySKUsAsync(IEnumerable<string> skus);
}
