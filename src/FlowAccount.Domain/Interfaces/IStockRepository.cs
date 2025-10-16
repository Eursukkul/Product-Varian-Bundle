using FlowAccount.Domain.Entities;

namespace FlowAccount.Domain.Interfaces;

/// <summary>
/// Stock Repository Interface
/// </summary>
public interface IStockRepository : IRepository<Stock>
{
    Task<Stock?> GetStockAsync(int warehouseId, string itemType, int itemId);

    Task<IEnumerable<Stock>> GetStocksByItemAsync(string itemType, int itemId);

    Task<IEnumerable<Stock>> GetStocksByWarehouseAsync(int warehouseId);

    Task UpdateStockQuantityAsync(int warehouseId, string itemType, int itemId, int quantity);

    Task<int> GetAvailableQuantityAsync(int warehouseId, string itemType, int itemId);
}
