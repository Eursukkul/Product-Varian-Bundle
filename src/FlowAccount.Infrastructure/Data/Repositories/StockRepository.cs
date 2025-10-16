using FlowAccount.Domain.Entities;
using FlowAccount.Domain.Interfaces;
using FlowAccount.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowAccount.Infrastructure.Data.Repositories;

public class StockRepository : Repository<Stock>, IStockRepository
{
    public StockRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Stock?> GetStockAsync(int warehouseId, string itemType, int itemId)
    {
        return await _dbSet
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s =>
                s.WarehouseId == warehouseId &&
                s.ItemType == itemType &&
                s.ItemId == itemId);
    }

    public async Task<IEnumerable<Stock>> GetStocksByItemAsync(string itemType, int itemId)
    {
        return await _dbSet
            .Include(s => s.Warehouse)
            .Where(s => s.ItemType == itemType && s.ItemId == itemId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Stock>> GetStocksByWarehouseAsync(int warehouseId)
    {
        return await _dbSet
            .Where(s => s.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<Dictionary<int, int>> GetBundleAvailableStockAsync(int bundleId, int warehouseId)
    {
        // Get bundle items
        var bundleItems = await _context.BundleItems
            .Where(bi => bi.BundleId == bundleId)
            .ToListAsync();

        var result = new Dictionary<int, int>();

        foreach (var item in bundleItems)
        {
            var stock = await GetStockAsync(warehouseId, item.ItemType, item.ItemId);

            if (stock == null)
            {
                result[item.ItemId] = 0;
                continue;
            }

            // Calculate how many bundles can be made with this item
            var possibleBundles = stock.Quantity / item.Quantity;
            result[item.ItemId] = possibleBundles;
        }

        return result;
    }

    public async Task UpdateStockQuantityAsync(int warehouseId, string itemType, int itemId, int quantity)
    {
        var stock = await GetStockAsync(warehouseId, itemType, itemId);

        if (stock == null)
        {
            stock = new Stock
            {
                WarehouseId = warehouseId,
                ItemType = itemType,
                ItemId = itemId,
                Quantity = quantity,
                LastUpdated = DateTime.UtcNow
            };

            await _dbSet.AddAsync(stock);
        }
        else
        {
            stock.Quantity = quantity;
            stock.LastUpdated = DateTime.UtcNow;
            _dbSet.Update(stock);
        }
    }

    public async Task<int> GetAvailableQuantityAsync(int warehouseId, string itemType, int itemId)
    {
        var stock = await GetStockAsync(warehouseId, itemType, itemId);
        return stock?.Quantity ?? 0;
    }
}
