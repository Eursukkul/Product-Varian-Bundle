using FlowAccount.Domain.Entities;
using FlowAccount.Domain.Interfaces;
using FlowAccount.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowAccount.Infrastructure.Data.Repositories;

public class BundleRepository : Repository<Bundle>, IBundleRepository
{
    public BundleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Bundle?> GetBundleWithItemsAsync(int id)
    {
        // ✅ OPTIMIZATION: AsNoTracking for read-only
        return await _dbSet
            .AsNoTracking()
            .Include(b => b.BundleItems)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bundle?> GetByIdWithDetailsAsync(int id)
    {
        return await GetBundleWithItemsAsync(id);
    }

    public async Task<IEnumerable<Bundle>> GetActiveBundlesAsync()
    {
        // ✅ OPTIMIZATION: AsNoTracking for read-only
        return await _dbSet
            .AsNoTracking()
            .Include(b => b.BundleItems)
            .Where(b => b.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Bundle>> GetAllWithItemsAsync()
    {
        return await GetActiveBundlesAsync();
    }

    public async Task<IEnumerable<BundleItem>> GetBundleItemsAsync(int bundleId)
    {
        // ✅ OPTIMIZATION: AsNoTracking for read-only
        return await _context.BundleItems
            .AsNoTracking()
            .Where(bi => bi.BundleId == bundleId)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _dbSet.AsNoTracking().Where(b => b.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(b => b.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Bundle>> GetBundlesContainingItemAsync(string itemType, int itemId)
    {
        // ✅ OPTIMIZATION: AsNoTracking for read-only
        return await _dbSet
            .AsNoTracking()
            .Include(b => b.BundleItems)
            .Where(b => b.BundleItems.Any(bi => bi.ItemType == itemType && bi.ItemId == itemId))
            .ToListAsync();
    }
}
