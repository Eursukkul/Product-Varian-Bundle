using FlowAccount.Domain.Entities;

namespace FlowAccount.Domain.Interfaces;

/// <summary>
/// Bundle Repository Interface
/// </summary>
public interface IBundleRepository : IRepository<Bundle>
{
    Task<Bundle?> GetBundleWithItemsAsync(int id);

    Task<IEnumerable<Bundle>> GetActiveBundlesAsync();

    Task<IEnumerable<BundleItem>> GetBundleItemsAsync(int bundleId);
}
