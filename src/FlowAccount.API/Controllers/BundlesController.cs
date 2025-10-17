using FlowAccount.Application.DTOs.Bundle;
using FlowAccount.Application.DTOs.Common;
using FlowAccount.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace FlowAccount.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Product Bundle Management - Create bundles, calculate stock, and manage bundle sales with bottleneck detection")]
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
    [Tags("Bundles")]
    [SwaggerOperation(
        Summary = "Get all product bundles",
        Description = "Retrieves a list of all bundles with their component items, stock availability, and pricing information.",
        OperationId = "GetAllBundles"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Bundles retrieved successfully", typeof(ResponseDto<List<BundleDto>>))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FlowAccount.API.Examples.BundleListResponseExample))]
    [ProducesResponseType(typeof(ResponseDto<List<BundleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving bundles. Please try again later.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            );
        }
    }

    /// <summary>
    /// Get bundle by ID
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <returns>Bundle details</returns>
    [HttpGet("{id}")]
    [Tags("Bundles")]
    [SwaggerOperation(
        Summary = "Get bundle by ID",
        Description = "Retrieves detailed information about a specific bundle including all component items, stock levels, and bottleneck analysis.",
        OperationId = "GetBundleById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Bundle retrieved successfully", typeof(ResponseDto<BundleDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Bundle not found", typeof(ProblemDetails))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FlowAccount.API.Examples.BundleResponseExample))]
    [ProducesResponseType(typeof(ResponseDto<BundleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDto<BundleDto>>> GetBundleById(int id)
    {
        try
        {
            var bundle = await _bundleService.GetBundleByIdAsync(id);
            if (bundle == null)
            {
                return Problem(
                    title: "Bundle Not Found",
                    detail: $"Bundle with ID {id} was not found.",
                    statusCode: StatusCodes.Status404NotFound,
                    instance: HttpContext.Request.Path
                );
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
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving the bundle. Please try again later.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            );
        }
    }

    /// <summary>
    /// Create a new bundle with mixed products and variants
    /// </summary>
    /// <param name="request">Bundle creation details</param>
    /// <returns>Created bundle</returns>
    /// <remarks>
    /// Creates a product bundle containing multiple items (product variants or product masters).
    /// 
    /// **Example Request:**
    /// 
    ///     POST /api/bundles
    ///     {
    ///         "name": "Summer Collection Bundle",
    ///         "description": "3-pack summer t-shirts",
    ///         "price": 799.00,
    ///         "isActive": true,
    ///         "items": [
    ///             {
    ///                 "itemType": "Variant",
    ///                 "itemId": 56,
    ///                 "quantity": 1
    ///             }
    ///         ]
    ///     }
    ///     
    /// **Item Types:**
    /// - **Variant**: Specific product variant (e.g., T-Shirt-M-Blue)
    /// - **Product**: Product master (any variant can fulfill)
    /// 
    /// </remarks>
    [HttpPost]
    [Tags("Bundles")]
    [SwaggerOperation(
        Summary = "Create a new product bundle",
        Description = "Creates a bundle combining multiple product variants or product masters. Bundles are sold as complete packages with automatic stock deduction for all components.",
        OperationId = "CreateBundle"
    )]
    [SwaggerRequestExample(typeof(CreateBundleRequest), typeof(FlowAccount.API.Examples.CreateBundleRequestExample))]
    [SwaggerResponse(StatusCodes.Status201Created, "Bundle created successfully", typeof(ResponseDto<BundleDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data", typeof(ProblemDetails))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(FlowAccount.API.Examples.BundleResponseExample))]
    [ProducesResponseType(typeof(ResponseDto<BundleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDto<BundleDto>>> CreateBundle(
        [FromBody] CreateBundleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Problem(
                    title: "Validation Error",
                    detail: "The request contains invalid data. Please check all required fields.",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
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
            return Problem(
                title: "Invalid Operation",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                instance: HttpContext.Request.Path
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bundle");
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while creating the bundle. Please try again later.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            );
        }
    }

    /// <summary>
    /// Update an existing bundle
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <param name="request">Updated bundle details</param>
    /// <returns>Updated bundle</returns>
    [HttpPut("{id}")]
    [Tags("Bundles")]
    [SwaggerOperation(
        Summary = "Update an existing bundle",
        Description = "Updates bundle information including name, description, price, active status, and component items.",
        OperationId = "UpdateBundle"
    )]
    [SwaggerRequestExample(typeof(CreateBundleRequest), typeof(FlowAccount.API.Examples.CreateBundleRequestExample))]
    [SwaggerResponse(StatusCodes.Status200OK, "Bundle updated successfully", typeof(ResponseDto<BundleDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Bundle not found", typeof(ProblemDetails))]
    [ProducesResponseType(typeof(ResponseDto<BundleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDto<BundleDto>>> UpdateBundle(
        int id,
        [FromBody] CreateBundleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Problem(
                    title: "Validation Error",
                    detail: "The request contains invalid data. Please check all required fields.",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
            }

            var result = await _bundleService.UpdateBundleAsync(id, request);
            if (result == null)
            {
                return Problem(
                    title: "Bundle Not Found",
                    detail: $"Bundle with ID {id} was not found.",
                    statusCode: StatusCodes.Status404NotFound,
                    instance: HttpContext.Request.Path
                );
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
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while updating the bundle. Please try again later.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            );
        }
    }

    /// <summary>
    /// Delete a bundle
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [Tags("Bundles", "Admin")]
    [SwaggerOperation(
        Summary = "Delete a bundle",
        Description = "Permanently deletes a bundle. This operation cannot be undone.",
        OperationId = "DeleteBundle"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Bundle deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Bundle not found", typeof(ProblemDetails))]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDto<object>>> DeleteBundle(int id)
    {
        try
        {
            var result = await _bundleService.DeleteBundleAsync(id);
            if (!result)
            {
                return Problem(
                    title: "Bundle Not Found",
                    detail: $"Bundle with ID {id} was not found.",
                    statusCode: StatusCodes.Status404NotFound,
                    instance: HttpContext.Request.Path
                );
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
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while deleting the bundle. Please try again later.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            );
        }
    }

    /// <summary>
    /// Calculate available stock for a bundle in a specific warehouse (STOCK LOGIC with Bottleneck Detection)
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <param name="request">Stock calculation parameters</param>
    /// <returns>Detailed stock calculation with bottleneck identification</returns>
    /// <remarks>
    /// **KEY FEATURE: Stock Logic with Bottleneck Detection**
    /// 
    /// Calculates how many bundles can be produced from available inventory
    /// and identifies which component is the limiting factor (bottleneck).
    /// 
    /// **Algorithm:**
    /// 1. For each item: `possible_bundles = available_stock ÷ required_quantity`
    /// 2. `max_bundles = MIN(all possible_bundles)`
    /// 3. Bottleneck items = items where `possible_bundles == max_bundles`
    /// 
    /// **Example:**
    /// - Item A: 50 available ÷ 1 required = 50 bundles possible
    /// - Item B: 15 available ÷ 1 required = 15 bundles possible ⚠️ BOTTLENECK
    /// - Item C: 100 available ÷ 2 required = 50 bundles possible
    /// - **Result:** Can make 15 bundles (limited by Item B)
    /// 
    /// </remarks>
    [HttpPost("{id}/calculate-stock")]
    [Tags("Bundles", "Stock")]
    [SwaggerOperation(
        Summary = "Calculate bundle stock with bottleneck detection",
        Description = "Analyzes available inventory for all bundle components and identifies which item limits production. Essential for inventory planning and order fulfillment.",
        OperationId = "CalculateBundleStock"
    )]
    [SwaggerRequestExample(typeof(CalculateBundleStockRequest), typeof(FlowAccount.API.Examples.CalculateBundleStockRequestExample))]
    [SwaggerResponse(StatusCodes.Status200OK, "Stock calculated successfully", typeof(ResponseDto<BundleStockCalculationResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Bundle or warehouse not found", typeof(ProblemDetails))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FlowAccount.API.Examples.BundleStockCalculationResponseExample))]
    [ProducesResponseType(typeof(ResponseDto<BundleStockCalculationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDto<BundleStockCalculationResponse>>> CalculateBundleStock(
        int id,
        [FromBody] CalculateBundleStockRequest request)
    {
        try
        {
            if (id != request.BundleId)
            {
                return Problem(
                    title: "ID Mismatch",
                    detail: "The bundle ID in the URL does not match the bundle ID in the request body.",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
            }

            if (!ModelState.IsValid)
            {
                return Problem(
                    title: "Validation Error",
                    detail: "The request contains invalid data. Please check all required fields.",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
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
            return Problem(
                title: "Not Found",
                detail: ex.Message,
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating bundle stock for {BundleId}", id);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while calculating bundle stock. Please try again later.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            );
        }
    }

    /// <summary>
    /// Sell a bundle and deduct stock from all components (TRANSACTION MANAGEMENT with Atomic Operations)
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <param name="request">Bundle sale details</param>
    /// <returns>Sale result with stock deduction details</returns>
    /// <remarks>
    /// **KEY FEATURE: Transaction Management with Atomic Operations**
    /// 
    /// Processes a bundle sale by deducting inventory from all component items
    /// with full transaction guarantees to ensure data consistency.
    /// 
    /// **Transaction Guarantees:**
    /// - **ATOMIC**: All stock deductions succeed or all are rolled back (no partial sales)
    /// - **VALIDATED**: Checks available stock before deduction (configurable via allowBackorder)
    /// - **TRACKED**: Records exact quantities before/after for each component
    /// - **ISOLATED**: Uses database transaction for consistency
    /// 
    /// **Process Flow:**
    /// 1. Validate bundle exists and is active
    /// 2. Calculate stock availability (if allowBackorder=false)
    /// 3. Begin database transaction
    /// 4. Deduct stock from all components
    /// 5. Record transaction details
    /// 6. Commit or rollback based on success
    /// 
    /// **Use Cases:**
    /// - Standard sales: `allowBackorder=false` ensures sufficient stock
    /// - Pre-orders: `allowBackorder=true` allows negative stock levels
    /// 
    /// </remarks>
    [HttpPost("{id}/sell")]
    [Tags("Bundles", "Stock", "Admin")]
    [SwaggerOperation(
        Summary = "Sell bundle with atomic stock deduction",
        Description = "Processes a bundle sale with full ACID transaction guarantees. Deducts inventory from all components atomically - all succeed or all rollback. Tracks before/after stock levels for audit trail.",
        OperationId = "SellBundle"
    )]
    [SwaggerRequestExample(typeof(SellBundleRequest), typeof(FlowAccount.API.Examples.SellBundleRequestExample))]
    [SwaggerResponse(StatusCodes.Status200OK, "Bundle sold successfully", typeof(ResponseDto<SellBundleResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or insufficient stock", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Bundle or warehouse not found", typeof(ProblemDetails))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FlowAccount.API.Examples.SellBundleResponseExample))]
    [ProducesResponseType(typeof(ResponseDto<SellBundleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDto<SellBundleResponse>>> SellBundle(
        int id,
        [FromBody] SellBundleRequest request)
    {
        try
        {
            if (id != request.BundleId)
            {
                return Problem(
                    title: "ID Mismatch",
                    detail: "The bundle ID in the URL does not match the bundle ID in the request body.",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
            }

            if (!ModelState.IsValid)
            {
                return Problem(
                    title: "Validation Error",
                    detail: "The request contains invalid data. Please check all required fields.",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
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
            return Problem(
                title: "Insufficient Stock",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                instance: HttpContext.Request.Path
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Bundle or warehouse not found");
            return Problem(
                title: "Not Found",
                detail: ex.Message,
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selling bundle {BundleId}", id);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while processing the sale. The transaction has been rolled back.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            );
        }
    }
}
