using FlowAccount.Domain.Entities;
using FlowAccount.Domain.Interfaces;
using FlowAccount.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowAccount.Infrastructure.Data.Repositories;

public class ProductRepository : Repository<ProductMaster>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProductMaster?> GetProductWithVariantsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.Attributes)
                    .ThenInclude(pva => pva.VariantOptionValue)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProductMaster?> GetProductWithOptionsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.VariantOptions)
                .ThenInclude(vo => vo.Values)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsBySKUAsync(string sku, int? excludeProductId = null)
    {
        // Check if any variant of any product (except excludeProductId) has this SKU
        var query = _context.ProductVariants
            .Where(pv => pv.SKU == sku);

        if (excludeProductId.HasValue)
        {
            query = query.Where(pv => pv.ProductMasterId != excludeProductId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<ProductMaster>> GetActiveProductsAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.ProductVariants)
            .Where(p => p.IsActive)
            .ToListAsync();
    }
}
