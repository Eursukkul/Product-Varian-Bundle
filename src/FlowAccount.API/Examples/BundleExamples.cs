using FlowAccount.Application.DTOs.Bundle;
using FlowAccount.Application.DTOs.Common;
using Swashbuckle.AspNetCore.Filters;

namespace FlowAccount.API.Examples;

// ============================================
// REQUEST EXAMPLES
// ============================================

/// <summary>
/// Example request for creating a product bundle
/// </summary>
public class CreateBundleRequestExample : IExamplesProvider<CreateBundleRequest>
{
    public CreateBundleRequest GetExamples()
    {
        return new CreateBundleRequest
        {
            Name = "Summer T-Shirt Bundle",
            Description = "3-pack summer t-shirts in different colors - Perfect for vacation!",
            Price = 799.00m,
            IsActive = true,
            Items = new List<CreateBundleItemRequest>
            {
                new CreateBundleItemRequest
                {
                    ItemType = "Variant",
                    ItemId = 56,
                    Quantity = 1
                },
                new CreateBundleItemRequest
                {
                    ItemType = "Variant",
                    ItemId = 57,
                    Quantity = 1
                },
                new CreateBundleItemRequest
                {
                    ItemType = "Variant",
                    ItemId = 58,
                    Quantity = 1
                }
            }
        };
    }
}

/// <summary>
/// Example request for calculating bundle stock
/// </summary>
public class CalculateBundleStockRequestExample : IExamplesProvider<CalculateBundleStockRequest>
{
    public CalculateBundleStockRequest GetExamples()
    {
        return new CalculateBundleStockRequest
        {
            BundleId = 1,
            WarehouseId = 1
        };
    }
}

/// <summary>
/// Example request for selling a bundle
/// </summary>
public class SellBundleRequestExample : IExamplesProvider<SellBundleRequest>
{
    public SellBundleRequest GetExamples()
    {
        return new SellBundleRequest
        {
            BundleId = 1,
            Quantity = 5,
            WarehouseId = 1
        };
    }
}

// ============================================
// RESPONSE EXAMPLES
// ============================================

/// <summary>
/// Example response for getting all bundles
/// </summary>
public class BundleListResponseExample : IExamplesProvider<ResponseDto<List<BundleDto>>>
{
    public ResponseDto<List<BundleDto>> GetExamples()
    {
        return new ResponseDto<List<BundleDto>>
        {
            Success = true,
            Message = "Bundles retrieved successfully",
            Data = new List<BundleDto>
            {
                new BundleDto
                {
                    Id = 1,
                    Name = "Summer T-Shirt Bundle",
                    Description = "3-pack summer t-shirts in different colors",
                    Price = 799.00m,
                    IsActive = true,
                    CreatedAt = DateTime.Parse("2025-10-15T10:30:00Z"),
                    Items = new List<BundleItemDto>
                    {
                        new BundleItemDto
                        {
                            Id = 1,
                            ItemType = "Variant",
                            ItemId = 56,
                            ItemName = "Ultimate T-Shirt - XS - Black - Cotton",
                            ItemSKU = "ULTIMATE-XS-BLACK-COTTON",
                            Quantity = 1,
                            AvailableStock = 50,
                            PossibleBundles = 50
                        },
                        new BundleItemDto
                        {
                            Id = 2,
                            ItemType = "Variant",
                            ItemId = 57,
                            ItemName = "Ultimate T-Shirt - XS - Black - Polyester",
                            ItemSKU = "ULTIMATE-XS-BLACK-POLYESTER",
                            Quantity = 1,
                            AvailableStock = 30,
                            PossibleBundles = 30
                        }
                    },
                    AvailableStock = 30,
                    StockMessage = "30 bundles available (limited by ULTIMATE-XS-BLACK-POLYESTER)"
                }
            }
        };
    }
}

/// <summary>
/// Example response for getting a single bundle
/// </summary>
public class BundleResponseExample : IExamplesProvider<ResponseDto<BundleDto>>
{
    public ResponseDto<BundleDto> GetExamples()
    {
        return new ResponseDto<BundleDto>
        {
            Success = true,
            Message = "Bundle retrieved successfully",
            Data = new BundleDto
            {
                Id = 1,
                Name = "Summer T-Shirt Bundle",
                Description = "3-pack summer t-shirts in different colors - Perfect for vacation!",
                Price = 799.00m,
                IsActive = true,
                CreatedAt = DateTime.Parse("2025-10-15T10:30:00Z"),
                Items = new List<BundleItemDto>
                {
                    new BundleItemDto
                    {
                        Id = 1,
                        ItemType = "Variant",
                        ItemId = 56,
                        ItemName = "Ultimate T-Shirt - XS - Black - Cotton",
                        ItemSKU = "ULTIMATE-XS-BLACK-COTTON",
                        Quantity = 1,
                        AvailableStock = 50,
                        PossibleBundles = 50
                    },
                    new BundleItemDto
                    {
                        Id = 2,
                        ItemType = "Variant",
                        ItemId = 57,
                        ItemName = "Ultimate T-Shirt - XS - Black - Polyester",
                        ItemSKU = "ULTIMATE-XS-BLACK-POLYESTER",
                        Quantity = 1,
                        AvailableStock = 30,
                        PossibleBundles = 30
                    },
                    new BundleItemDto
                    {
                        Id = 3,
                        ItemType = "Variant",
                        ItemId = 58,
                        ItemName = "Ultimate T-Shirt - XS - White - Cotton",
                        ItemSKU = "ULTIMATE-XS-WHITE-COTTON",
                        Quantity = 1,
                        AvailableStock = 45,
                        PossibleBundles = 45
                    }
                },
                AvailableStock = 30,
                StockMessage = "30 bundles available (limited by ULTIMATE-XS-BLACK-POLYESTER)"
            }
        };
    }
}

/// <summary>
/// Example response for bundle stock calculation with bottleneck detection
/// </summary>
public class BundleStockCalculationResponseExample : IExamplesProvider<ResponseDto<BundleStockCalculationResponse>>
{
    public ResponseDto<BundleStockCalculationResponse> GetExamples()
    {
        return new ResponseDto<BundleStockCalculationResponse>
        {
            Success = true,
            Message = "Bundle can produce 15 units",
            Data = new BundleStockCalculationResponse
            {
                BundleId = 1,
                BundleName = "Summer T-Shirt Bundle",
                WarehouseId = 1,
                WarehouseName = "Main Warehouse",
                MaxAvailableBundles = 15,
                ItemsStockBreakdown = new List<BundleItemStockInfo>
                {
                    new BundleItemStockInfo
                    {
                        ItemType = "Variant",
                        ItemId = 56,
                        ItemName = "Ultimate T-Shirt - M - Black - Cotton",
                        ItemSKU = "ULTIMATE-M-BLACK-COTTON",
                        RequiredQuantity = 1,
                        AvailableQuantity = 50,
                        PossibleBundles = 50,
                        IsBottleneck = false
                    },
                    new BundleItemStockInfo
                    {
                        ItemType = "Variant",
                        ItemId = 57,
                        ItemName = "Ultimate T-Shirt - M - White - Cotton",
                        ItemSKU = "ULTIMATE-M-WHITE-COTTON",
                        RequiredQuantity = 1,
                        AvailableQuantity = 15,
                        PossibleBundles = 15,
                        IsBottleneck = true
                    },
                    new BundleItemStockInfo
                    {
                        ItemType = "Variant",
                        ItemId = 58,
                        ItemName = "Ultimate T-Shirt - M - Red - Cotton",
                        ItemSKU = "ULTIMATE-M-RED-COTTON",
                        RequiredQuantity = 1,
                        AvailableQuantity = 100,
                        PossibleBundles = 100,
                        IsBottleneck = false
                    }
                },
                Explanation = "Bundle availability limited by Ultimate T-Shirt - M - White - Cotton - only 15 units available",
                CalculatedAt = DateTime.Parse("2025-10-17T14:30:00Z")
            }
        };
    }
}

/// <summary>
/// Example response for selling a bundle
/// </summary>
public class SellBundleResponseExample : IExamplesProvider<ResponseDto<SellBundleResponse>>
{
    public ResponseDto<SellBundleResponse> GetExamples()
    {
        return new ResponseDto<SellBundleResponse>
        {
            Success = true,
            Message = "Bundle sold successfully. 10 bundles remaining",
            Data = new SellBundleResponse
            {
                Success = true,
                Message = "Bundle sold successfully",
                TransactionId = "TXN-20251017-001",
                BundleId = 1,
                BundleName = "Summer T-Shirt Bundle",
                QuantitySold = 5,
                TotalAmount = 3995.00m,
                StockDeductions = new List<StockDeductionInfo>
                {
                    new StockDeductionInfo
                    {
                        ItemType = "Variant",
                        ItemId = 56,
                        ItemName = "Ultimate T-Shirt - M - Black - Cotton",
                        QuantityDeducted = 5,
                        StockBefore = 50,
                        StockAfter = 45
                    },
                    new StockDeductionInfo
                    {
                        ItemType = "Variant",
                        ItemId = 57,
                        ItemName = "Ultimate T-Shirt - M - White - Cotton",
                        QuantityDeducted = 5,
                        StockBefore = 15,
                        StockAfter = 10
                    },
                    new StockDeductionInfo
                    {
                        ItemType = "Variant",
                        ItemId = 58,
                        ItemName = "Ultimate T-Shirt - M - Red - Cotton",
                        QuantityDeducted = 5,
                        StockBefore = 100,
                        StockAfter = 95
                    }
                },
                RemainingBundleStock = 10,
                TransactionDate = DateTime.Parse("2025-10-17T14:30:00Z")
            }
        };
    }
}
