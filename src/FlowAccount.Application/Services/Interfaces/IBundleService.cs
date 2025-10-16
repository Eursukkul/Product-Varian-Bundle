using FlowAccount.Application.DTOs.Bundle;

namespace FlowAccount.Application.Services.Interfaces;

public interface IBundleService
{
    Task<BundleDto?> GetBundleByIdAsync(int id);
    Task<IEnumerable<BundleDto>> GetAllBundlesAsync();
    Task<BundleOperationResponse> CreateBundleAsync(CreateBundleRequest request);
    Task<BundleOperationResponse> UpdateBundleAsync(int id, CreateBundleRequest request);
    Task<bool> DeleteBundleAsync(int id);

    /// <summary>
    /// Calculate available stock for a bundle (STOCK LOGIC)
    /// Returns the minimum possible bundles based on component stock
    /// </summary>
    Task<BundleStockCalculationResponse> CalculateBundleStockAsync(CalculateBundleStockRequest request);

    /// <summary>
    /// Sell a bundle with transaction management (TRANSACTION MANAGEMENT)
    /// Deducts stock from all components atomically
    /// </summary>
    Task<SellBundleResponse> SellBundleAsync(SellBundleRequest request);
}
