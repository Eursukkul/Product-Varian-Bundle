using AutoMapper;
using FlowAccount.Application.DTOs.Bundle;
using FlowAccount.Application.Services.Interfaces;
using FlowAccount.Domain.Entities;
using FlowAccount.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlowAccount.Application.Services;

public class BundleService : IBundleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BundleService> _logger;

    public BundleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BundleService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BundleDto?> GetBundleByIdAsync(int id)
    {
        var bundle = await _unitOfWork.Bundles.GetBundleWithItemsAsync(id);
        if (bundle == null) return null;

        var dto = _mapper.Map<BundleDto>(bundle);

        // Populate item names
        await PopulateItemNames(dto);

        return dto;
    }

    public async Task<IEnumerable<BundleDto>> GetAllBundlesAsync()
    {
        var bundles = await _unitOfWork.Bundles.GetActiveBundlesAsync();
        var dtos = _mapper.Map<IEnumerable<BundleDto>>(bundles);

        foreach (var dto in dtos)
        {
            await PopulateItemNames(dto);
        }

        return dtos;
    }

    public async Task<BundleOperationResponse> CreateBundleAsync(CreateBundleRequest request)
    {
        _logger.LogInformation(
            "Creating new bundle: Name={BundleName}, Price={Price}, ItemCount={ItemCount}",
            request.Name,
            request.Price,
            request.Items.Count);

        try
        {
            // Validate items exist
            await ValidateBundleItems(request.Items);

            // Create bundle
            var bundle = new Bundle
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Bundles.AddAsync(bundle);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogDebug("Bundle master created: BundleId={BundleId}", bundle.Id);

            // Create bundle items
            bundle.BundleItems = request.Items.Select(itemRequest => new BundleItem
            {
                BundleId = bundle.Id,
                ItemType = itemRequest.ItemType,
                ItemId = itemRequest.ItemId,
                Quantity = itemRequest.Quantity
            }).ToList();

            _unitOfWork.Bundles.Update(bundle);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully created bundle: BundleId={BundleId}, Name={BundleName}, ItemCount={ItemCount}",
                bundle.Id,
                bundle.Name,
                bundle.BundleItems.Count);

            // Return created bundle
            var createdBundle = await _unitOfWork.Bundles.GetBundleWithItemsAsync(bundle.Id);
            var dto = _mapper.Map<BundleDto>(createdBundle!);
            await PopulateItemNames(dto);

            return new BundleOperationResponse
            {
                Bundle = dto,
                Message = "Bundle created successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create bundle: Name={BundleName}, ItemCount={ItemCount}",
                request.Name,
                request.Items.Count);
            throw;
        }
    }

    public async Task<BundleOperationResponse> UpdateBundleAsync(int id, CreateBundleRequest request)
    {
        _logger.LogInformation(
            "Updating bundle: BundleId={BundleId}, Name={BundleName}, ItemCount={ItemCount}",
            id,
            request.Name,
            request.Items.Count);

        try
        {
            var bundle = await _unitOfWork.Bundles.GetBundleWithItemsAsync(id);
            if (bundle == null)
            {
                _logger.LogWarning("Bundle not found for update: BundleId={BundleId}", id);
                throw new KeyNotFoundException($"Bundle with ID {id} not found");
            }

            // Validate items exist
            await ValidateBundleItems(request.Items);

            await _unitOfWork.BeginTransactionAsync();
            _logger.LogDebug("Transaction started for bundle update");

            // Update bundle
            bundle.Name = request.Name;
            bundle.Description = request.Description;
            bundle.Price = request.Price;
            bundle.IsActive = request.IsActive;
            bundle.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Bundles.Update(bundle);

            // Remove old items and add new items
            bundle.BundleItems.Clear();
            bundle.BundleItems = request.Items.Select(itemRequest => new BundleItem
            {
                BundleId = bundle.Id,
                ItemType = itemRequest.ItemType,
                ItemId = itemRequest.ItemId,
                Quantity = itemRequest.Quantity
            }).ToList();

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Successfully updated bundle: BundleId={BundleId}, Name={BundleName}, ItemCount={ItemCount}",
                id,
                bundle.Name,
                bundle.BundleItems.Count);

            // Return updated bundle
            var updatedBundle = await _unitOfWork.Bundles.GetBundleWithItemsAsync(id);
            var dto = _mapper.Map<BundleDto>(updatedBundle!);
            await PopulateItemNames(dto);

            return new BundleOperationResponse
            {
                Bundle = dto,
                Message = "Bundle updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to update bundle: BundleId={BundleId}, Name={BundleName}",
                id,
                request.Name);

            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogWarning("Transaction rolled back for bundle update");
            throw;
        }
    }

    public async Task<bool> DeleteBundleAsync(int id)
    {
        _logger.LogInformation("Deleting bundle: BundleId={BundleId}", id);

        try
        {
            var bundle = await _unitOfWork.Bundles.GetByIdAsync(id);
            if (bundle == null)
            {
                _logger.LogWarning("Bundle not found for deletion: BundleId={BundleId}", id);
                return false;
            }

            _unitOfWork.Bundles.Remove(bundle);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted bundle: BundleId={BundleId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete bundle: BundleId={BundleId}", id);
            throw;
        }
    }

    /// <summary>
    /// STOCK LOGIC: Calculate available bundle stock
    /// Returns the minimum possible bundles based on component stock
    /// </summary>
    public async Task<BundleStockCalculationResponse> CalculateBundleStockAsync(CalculateBundleStockRequest request)
    {
        _logger.LogInformation(
            "Calculating bundle stock: BundleId={BundleId}, WarehouseId={WarehouseId}",
            request.BundleId,
            request.WarehouseId);

        try
        {
            // Get bundle with items
            var bundle = await _unitOfWork.Bundles.GetBundleWithItemsAsync(request.BundleId);
            if (bundle == null)
            {
                _logger.LogWarning(
                    "Bundle not found for stock calculation: BundleId={BundleId}",
                    request.BundleId);
                throw new KeyNotFoundException($"Bundle with ID {request.BundleId} not found");
            }

            _logger.LogDebug(
                "Bundle loaded: Name={BundleName}, ItemCount={ItemCount}",
                bundle.Name,
                bundle.BundleItems.Count);

            var itemsStockBreakdown = new List<BundleItemStockInfo>();
            int minPossibleBundles = int.MaxValue;

            // Calculate stock for each item
            foreach (var item in bundle.BundleItems)
            {
                var availableQuantity = await _unitOfWork.Stocks.GetAvailableQuantityAsync(
                    request.WarehouseId,
                    item.ItemType,
                    item.ItemId);

                var possibleBundles = availableQuantity / item.Quantity;

                _logger.LogDebug(
                    "Stock check: ItemType={ItemType}, ItemId={ItemId}, Available={Available}, Required={Required}, PossibleBundles={PossibleBundles}",
                    item.ItemType,
                    item.ItemId,
                    availableQuantity,
                    item.Quantity,
                    possibleBundles);

                // Get item name
                var itemName = await GetItemName(item.ItemType, item.ItemId);
                var itemSKU = item.ItemType == "Variant" ? await GetItemSKU(item.ItemId) : null;

                var stockInfo = new BundleItemStockInfo
                {
                    ItemType = item.ItemType,
                    ItemId = item.ItemId,
                    ItemName = itemName,
                    ItemSKU = itemSKU,
                    RequiredQuantity = item.Quantity,
                    AvailableQuantity = availableQuantity,
                    PossibleBundles = possibleBundles,
                    IsBottleneck = false // Will be set later
                };

                itemsStockBreakdown.Add(stockInfo);

                // Track minimum
                if (possibleBundles < minPossibleBundles)
                {
                    minPossibleBundles = possibleBundles;
                }
            }

            // Mark bottleneck items
            foreach (var stockInfo in itemsStockBreakdown)
            {
                if (stockInfo.PossibleBundles == minPossibleBundles)
                {
                    stockInfo.IsBottleneck = true;
                    _logger.LogWarning(
                        "Bottleneck detected: ItemName={ItemName}, SKU={SKU}, Available={Available}, Required={Required}",
                        stockInfo.ItemName,
                        stockInfo.ItemSKU,
                        stockInfo.AvailableQuantity,
                        stockInfo.RequiredQuantity);
                }
            }

            // Build explanation
            var bottleneckItems = itemsStockBreakdown.Where(s => s.IsBottleneck).ToList();
            var explanation = $"Bundle '{bundle.Name}' can be sold {minPossibleBundles} times. ";

            if (bottleneckItems.Any())
            {
                var bottleneckNames = string.Join(", ", bottleneckItems.Select(b => $"{b.ItemName} ({b.AvailableQuantity} available, {b.RequiredQuantity} required)"));
                explanation += $"Limited by: {bottleneckNames}";
            }

            _logger.LogInformation(
                "Stock calculation completed: BundleId={BundleId}, MaxAvailable={MaxAvailable}, BottleneckCount={BottleneckCount}",
                bundle.Id,
                minPossibleBundles,
                bottleneckItems.Count);

            return new BundleStockCalculationResponse
            {
                BundleId = bundle.Id,
                BundleName = bundle.Name,
                WarehouseId = request.WarehouseId,
                WarehouseName = $"Warehouse {request.WarehouseId}",
                MaxAvailableBundles = minPossibleBundles,
                ItemsStockBreakdown = itemsStockBreakdown,
                Explanation = explanation,
                CalculatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to calculate bundle stock: BundleId={BundleId}, WarehouseId={WarehouseId}",
                request.BundleId,
                request.WarehouseId);
            throw;
        }
    }

    /// <summary>
    /// TRANSACTION MANAGEMENT: Sell bundle with atomic stock deduction
    /// </summary>
    public async Task<SellBundleResponse> SellBundleAsync(SellBundleRequest request)
    {
        var transactionId = Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Starting bundle sale transaction: TransactionId={TransactionId}, BundleId={BundleId}, Quantity={Quantity}, WarehouseId={WarehouseId}, AllowBackorder={AllowBackorder}",
            transactionId,
            request.BundleId,
            request.Quantity,
            request.WarehouseId,
            request.AllowBackorder);

        try
        {
            // Calculate available stock
            var stockCalculation = await CalculateBundleStockAsync(new CalculateBundleStockRequest
            {
                BundleId = request.BundleId,
                WarehouseId = request.WarehouseId
            });

            _logger.LogDebug(
                "Stock calculation completed: AvailableBundles={AvailableBundles}, Requested={Requested}",
                stockCalculation.MaxAvailableBundles,
                request.Quantity);

            // Check if sufficient stock
            if (stockCalculation.MaxAvailableBundles < request.Quantity && !request.AllowBackorder)
            {
                _logger.LogWarning(
                    "Insufficient stock for bundle sale: BundleId={BundleId}, Available={Available}, Requested={Requested}",
                    request.BundleId,
                    stockCalculation.MaxAvailableBundles,
                    request.Quantity);

                throw new InvalidOperationException(
                    $"Insufficient stock. Available: {stockCalculation.MaxAvailableBundles}, Requested: {request.Quantity}");
            }

            var bundle = await _unitOfWork.Bundles.GetBundleWithItemsAsync(request.BundleId);
            if (bundle == null)
            {
                _logger.LogWarning("Bundle not found for sale: BundleId={BundleId}", request.BundleId);
                throw new KeyNotFoundException($"Bundle with ID {request.BundleId} not found");
            }

            await _unitOfWork.BeginTransactionAsync();
            _logger.LogDebug("Transaction started: TransactionId={TransactionId}", transactionId);

            var stockDeductions = new List<StockDeductionInfo>();

            // Deduct stock for each item
            foreach (var item in bundle.BundleItems)
            {
                var quantityToDeduct = item.Quantity * request.Quantity;

                // Get current stock
                var currentStock = await _unitOfWork.Stocks.GetAvailableQuantityAsync(
                    request.WarehouseId,
                    item.ItemType,
                    item.ItemId);

                // Deduct stock
                var newStock = currentStock - quantityToDeduct;
                await _unitOfWork.Stocks.UpdateStockQuantityAsync(
                    request.WarehouseId,
                    item.ItemType,
                    item.ItemId,
                    newStock);

                _logger.LogDebug(
                    "Stock deducted: ItemType={ItemType}, ItemId={ItemId}, Before={Before}, Deducted={Deducted}, After={After}",
                    item.ItemType,
                    item.ItemId,
                    currentStock,
                    quantityToDeduct,
                    newStock);

                // Record deduction
                var itemName = await GetItemName(item.ItemType, item.ItemId);
                stockDeductions.Add(new StockDeductionInfo
                {
                    ItemType = item.ItemType,
                    ItemId = item.ItemId,
                    ItemName = itemName,
                    QuantityDeducted = quantityToDeduct,
                    StockBefore = currentStock,
                    StockAfter = newStock
                });
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Transaction committed: TransactionId={TransactionId}, BundleId={BundleId}, ItemsDeducted={ItemCount}",
                transactionId,
                bundle.Id,
                stockDeductions.Count);

            // Calculate remaining stock
            var remainingStock = await CalculateBundleStockAsync(new CalculateBundleStockRequest
            {
                BundleId = request.BundleId,
                WarehouseId = request.WarehouseId
            });

            var totalAmount = bundle.Price * request.Quantity;

            _logger.LogInformation(
                "Bundle sale completed successfully: TransactionId={TransactionId}, BundleId={BundleId}, BundleName={BundleName}, QuantitySold={QuantitySold}, TotalAmount={TotalAmount}, RemainingStock={RemainingStock}",
                transactionId,
                bundle.Id,
                bundle.Name,
                request.Quantity,
                totalAmount,
                remainingStock.MaxAvailableBundles);

            return new SellBundleResponse
            {
                Success = true,
                Message = "Bundle sold successfully",
                BundleId = bundle.Id,
                BundleName = bundle.Name,
                QuantitySold = request.Quantity,
                TotalAmount = bundle.Price * request.Quantity,
                StockDeductions = stockDeductions,
                RemainingBundleStock = remainingStock.MaxAvailableBundles,
                TransactionId = transactionId,
                TransactionDate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Bundle sale transaction failed: TransactionId={TransactionId}, BundleId={BundleId}, Quantity={Quantity}",
                transactionId,
                request.BundleId,
                request.Quantity);

            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogWarning("Transaction rolled back: TransactionId={TransactionId}", transactionId);
            throw;
        }
    }

    #region Helper Methods

    private async Task ValidateBundleItems(List<CreateBundleItemRequest> items)
    {
        foreach (var item in items)
        {
            if (item.ItemType == "Product")
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ItemId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {item.ItemId} not found");
                }
            }
            else if (item.ItemType == "Variant")
            {
                var variant = await _unitOfWork.Variants.GetByIdAsync(item.ItemId);
                if (variant == null)
                {
                    throw new KeyNotFoundException($"Variant with ID {item.ItemId} not found");
                }
            }
            else
            {
                throw new ArgumentException($"Invalid ItemType: {item.ItemType}. Must be 'Product' or 'Variant'");
            }
        }
    }

    private async Task PopulateItemNames(BundleDto dto)
    {
        foreach (var item in dto.Items)
        {
            item.ItemName = await GetItemName(item.ItemType, item.ItemId);

            if (item.ItemType == "Variant")
            {
                item.ItemSKU = await GetItemSKU(item.ItemId);
            }
        }
    }

    private async Task<string> GetItemName(string itemType, int itemId)
    {
        if (itemType == "Product")
        {
            var product = await _unitOfWork.Products.GetByIdAsync(itemId);
            return product?.Name ?? "Unknown";
        }
        else if (itemType == "Variant")
        {
            var variant = await _unitOfWork.Variants.GetByIdAsync(itemId);
            if (variant != null)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(variant.ProductMasterId);
                return $"{product?.Name} - {variant.SKU}";
            }
        }

        return "Unknown";
    }

    private async Task<string?> GetItemSKU(int variantId)
    {
        var variant = await _unitOfWork.Variants.GetByIdAsync(variantId);
        return variant?.SKU;
    }

    #endregion
}
