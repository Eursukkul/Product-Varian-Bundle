using FlowAccount.Application.DTOs.Bundle;
using FlowAccount.Application.DTOs.Common;
using FlowAccount.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlowAccount.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BundlesController : ControllerBase
{
    private readonly IBundleService _bundleService;
    private readonly ILogger<BundlesController> _logger;

    public BundlesController(
        IBundleService bundleService,
        ILogger<BundlesController> logger)
    {
        _bundleService = bundleService;
        _logger = logger;
    }

    /// <summary>
    /// Get all bundles with their items
    /// </summary>
    /// <returns>List of bundles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ResponseDto<List<BundleDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<List<BundleDto>>>> GetAllBundles()
    {
        try
        {
            var bundles = await _bundleService.GetAllBundlesAsync();
            return Ok(new ResponseDto<List<BundleDto>>
            {
                Success = true,
                Message = "Bundles retrieved successfully",
                Data = bundles.ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bundles");
            return StatusCode(500, new ResponseDto<List<BundleDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving bundles",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Get bundle by ID
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <returns>Bundle details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDto<BundleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<BundleDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<BundleDto>>> GetBundleById(int id)
    {
        try
        {
            var bundle = await _bundleService.GetBundleByIdAsync(id);
            if (bundle == null)
            {
                return NotFound(new ResponseDto<BundleDto>
                {
                    Success = false,
                    Message = $"Bundle with ID {id} not found"
                });
            }

            return Ok(new ResponseDto<BundleDto>
            {
                Success = true,
                Message = "Bundle retrieved successfully",
                Data = bundle
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bundle {BundleId}", id);
            return StatusCode(500, new ResponseDto<BundleDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the bundle",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Create a new bundle with mixed products and variants
    /// </summary>
    /// <param name="request">Bundle creation details</param>
    /// <returns>Created bundle</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/bundles
    ///     {
    ///         "name": "Summer Collection Bundle",
    ///         "sku": "BUNDLE-SUMMER-001",
    ///         "bundlePrice": 799.00,
    ///         "isActive": true,
    ///         "items": [
    ///             {
    ///                 "itemType": "Product",
    ///                 "itemId": 5,         // ProductMaster ID
    ///                 "quantity": 2
    ///             },
    ///             {
    ///                 "itemType": "Variant",
    ///                 "itemId": 123,       // ProductVariant ID (T-Shirt-M-Blue)
    ///                 "quantity": 1
    ///             },
    ///             {
    ///                 "itemType": "Variant",
    ///                 "itemId": 127,       // ProductVariant ID (Shorts-L-Black)
    ///                 "quantity": 1
    ///             }
    ///         ]
    ///     }
    ///     
    /// Bundle can contain:
    /// - Product Master (any variant of that product can fulfill)
    /// - Specific Variants (must be that exact variant)
    /// 
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<BundleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseDto<BundleDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseDto<BundleDto>>> CreateBundle(
        [FromBody] CreateBundleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<BundleDto>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var result = await _bundleService.CreateBundleAsync(request);
            return CreatedAtAction(
                nameof(GetBundleById),
                new { id = result.Bundle.Id },
                new ResponseDto<BundleDto>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result.Bundle
                });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid bundle creation request");
            return BadRequest(new ResponseDto<BundleDto>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bundle");
            return StatusCode(500, new ResponseDto<BundleDto>
            {
                Success = false,
                Message = "An error occurred while creating the bundle",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Update an existing bundle
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <param name="request">Updated bundle details</param>
    /// <returns>Updated bundle</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResponseDto<BundleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<BundleDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<BundleDto>>> UpdateBundle(
        int id,
        [FromBody] CreateBundleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<BundleDto>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var result = await _bundleService.UpdateBundleAsync(id, request);
            if (result == null)
            {
                return NotFound(new ResponseDto<BundleDto>
                {
                    Success = false,
                    Message = $"Bundle with ID {id} not found"
                });
            }

            return Ok(new ResponseDto<BundleDto>
            {
                Success = true,
                Message = result.Message,
                Data = result.Bundle
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bundle {BundleId}", id);
            return StatusCode(500, new ResponseDto<BundleDto>
            {
                Success = false,
                Message = "An error occurred while updating the bundle",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Delete a bundle
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<object>>> DeleteBundle(int id)
    {
        try
        {
            var result = await _bundleService.DeleteBundleAsync(id);
            if (!result)
            {
                return NotFound(new ResponseDto<object>
                {
                    Success = false,
                    Message = $"Bundle with ID {id} not found"
                });
            }

            return Ok(new ResponseDto<object>
            {
                Success = true,
                Message = "Bundle deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bundle {BundleId}", id);
            return StatusCode(500, new ResponseDto<object>
            {
                Success = false,
                Message = "An error occurred while deleting the bundle",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Calculate available stock for a bundle in a specific warehouse (STOCK LOGIC)
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <param name="request">Stock calculation parameters</param>
    /// <returns>Detailed stock calculation with bottleneck identification</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/bundles/10/calculate-stock
    ///     {
    ///         "bundleId": 10,
    ///         "warehouseId": 1
    ///     }
    ///     
    /// Sample response:
    /// 
    ///     {
    ///         "maxAvailableBundles": 15,
    ///         "itemsStockBreakdown": [
    ///             {
    ///                 "itemName": "T-Shirt (M, Blue)",
    ///                 "itemSku": "TS-001-M-Blue",
    ///                 "requiredQuantity": 1,
    ///                 "availableQuantity": 50,
    ///                 "possibleBundles": 50,
    ///                 "isBottleneck": false
    ///             },
    ///             {
    ///                 "itemName": "Shorts (L, Black)",
    ///                 "itemSku": "SHORT-001-L-Black",
    ///                 "requiredQuantity": 1,
    ///                 "availableQuantity": 15,
    ///                 "possibleBundles": 15,
    ///                 "isBottleneck": true
    ///             },
    ///             {
    ///                 "itemName": "Hat",
    ///                 "itemSku": "HAT-001",
    ///                 "requiredQuantity": 2,
    ///                 "availableQuantity": 100,
    ///                 "possibleBundles": 50,
    ///                 "isBottleneck": false
    ///             }
    ///         ],
    ///         "warehouseName": "Main Warehouse",
    ///         "explanation": "Bundle availability limited by Shorts (L, Black) - only 15 units available"
    ///     }
    ///     
    /// Algorithm:
    /// - For each item, calculate: possible_bundles = available_stock / required_quantity
    /// - Max available bundles = MIN(all possible_bundles)
    /// - Bottleneck items = items where possible_bundles equals max_available_bundles
    /// 
    /// </remarks>
    [HttpPost("{id}/calculate-stock")]
    [ProducesResponseType(typeof(ResponseDto<BundleStockCalculationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<BundleStockCalculationResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<BundleStockCalculationResponse>>> CalculateBundleStock(
        int id,
        [FromBody] CalculateBundleStockRequest request)
    {
        try
        {
            if (id != request.BundleId)
            {
                return BadRequest(new ResponseDto<BundleStockCalculationResponse>
                {
                    Success = false,
                    Message = "Bundle ID mismatch"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<BundleStockCalculationResponse>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var response = await _bundleService.CalculateBundleStockAsync(request);
            return Ok(new ResponseDto<BundleStockCalculationResponse>
            {
                Success = true,
                Message = $"Bundle can produce {response.MaxAvailableBundles} units",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Bundle or warehouse not found");
            return NotFound(new ResponseDto<BundleStockCalculationResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating bundle stock for {BundleId}", id);
            return StatusCode(500, new ResponseDto<BundleStockCalculationResponse>
            {
                Success = false,
                Message = "An error occurred while calculating bundle stock",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Sell a bundle and deduct stock from all components (TRANSACTION MANAGEMENT)
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <param name="request">Bundle sale details</param>
    /// <returns>Sale result with stock deduction details</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/bundles/10/sell
    ///     {
    ///         "bundleId": 10,
    ///         "warehouseId": 1,
    ///         "quantity": 5,
    ///         "allowBackorder": false
    ///     }
    ///     
    /// Sample response:
    /// 
    ///     {
    ///         "transactionId": "TXN-20240116-001",
    ///         "stockDeductions": [
    ///             {
    ///                 "itemName": "T-Shirt (M, Blue)",
    ///                 "itemSku": "TS-001-M-Blue",
    ///                 "quantityDeducted": 5,
    ///                 "stockBefore": 50,
    ///                 "stockAfter": 45
    ///             },
    ///             {
    ///                 "itemName": "Shorts (L, Black)",
    ///                 "itemSku": "SHORT-001-L-Black",
    ///                 "quantityDeducted": 5,
    ///                 "stockBefore": 15,
    ///                 "stockAfter": 10
    ///             },
    ///             {
    ///                 "itemName": "Hat",
    ///                 "itemSku": "HAT-001",
    ///                 "quantityDeducted": 10,
    ///                 "stockBefore": 100,
    ///                 "stockAfter": 90
    ///             }
    ///         ],
    ///         "remainingBundleStock": 10
    ///     }
    ///     
    /// Transaction guarantees:
    /// - ATOMIC: All stock deductions succeed or all are rolled back
    /// - VALIDATED: Checks available stock before deduction (if allowBackorder=false)
    /// - TRACKED: Records exact quantities before/after for each component
    /// - ISOLATED: Uses database transaction for consistency
    /// 
    /// </remarks>
    [HttpPost("{id}/sell")]
    [ProducesResponseType(typeof(ResponseDto<SellBundleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<SellBundleResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseDto<SellBundleResponse>>> SellBundle(
        int id,
        [FromBody] SellBundleRequest request)
    {
        try
        {
            if (id != request.BundleId)
            {
                return BadRequest(new ResponseDto<SellBundleResponse>
                {
                    Success = false,
                    Message = "Bundle ID mismatch"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<SellBundleResponse>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var response = await _bundleService.SellBundleAsync(request);
            return Ok(new ResponseDto<SellBundleResponse>
            {
                Success = true,
                Message = $"Bundle sold successfully. {response.RemainingBundleStock} bundles remaining",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid bundle sale request");
            return BadRequest(new ResponseDto<SellBundleResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Bundle or warehouse not found");
            return NotFound(new ResponseDto<SellBundleResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selling bundle {BundleId}", id);
            return StatusCode(500, new ResponseDto<SellBundleResponse>
            {
                Success = false,
                Message = "An error occurred while processing the sale",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}
