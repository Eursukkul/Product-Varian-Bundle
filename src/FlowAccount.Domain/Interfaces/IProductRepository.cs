using FlowAccount.Domain.Entities;

namespace FlowAccount.Domain.Interfaces;

/// <summary>
/// Product Repository Interface
/// </summary>
public interface IProductRepository : IRepository<ProductMaster>
{
    Task<ProductMaster?> GetProductWithVariantsAsync(int id);

    Task<ProductMaster?> GetProductWithOptionsAsync(int id);

    Task<bool> ExistsBySKUAsync(string sku, int? excludeProductId = null);

    Task<IEnumerable<ProductMaster>> GetActiveProductsAsync();
}
