using FlowAccount.Domain.Entities;
using FlowAccount.Domain.Interfaces;
using FlowAccount.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowAccount.Infrastructure.Data.Repositories;

public class VariantRepository : Repository<ProductVariant>, IVariantRepository
{
    public VariantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsBySKUAsync(string sku, int? excludeVariantId = null)
    {
        var query = _dbSet.Where(pv => pv.SKU == sku);

        if (excludeVariantId.HasValue)
        {
            query = query.Where(pv => pv.Id != excludeVariantId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<ProductVariant>> GetVariantsByProductIdAsync(int productId)
    {
        return await _dbSet
            .Include(pv => pv.Attributes)
                .ThenInclude(pva => pva.VariantOptionValue)
                    .ThenInclude(vov => vov.VariantOption)
            .Where(pv => pv.ProductMasterId == productId && pv.IsActive)
            .ToListAsync();
    }

    public async Task<ProductVariant?> GetVariantWithAttributesAsync(int id)
    {
        return await _dbSet
            .Include(pv => pv.ProductMaster)
            .Include(pv => pv.Attributes)
                .ThenInclude(pva => pva.VariantOptionValue)
                    .ThenInclude(vov => vov.VariantOption)
            .FirstOrDefaultAsync(pv => pv.Id == id);
    }

    public async Task<IEnumerable<ProductVariant>> GetVariantsBySKUsAsync(IEnumerable<string> skus)
    {
        return await _dbSet
            .Include(pv => pv.ProductMaster)
            .Include(pv => pv.Attributes)
                .ThenInclude(pva => pva.VariantOptionValue)
            .Where(pv => skus.Contains(pv.SKU))
            .ToListAsync();
    }
}
