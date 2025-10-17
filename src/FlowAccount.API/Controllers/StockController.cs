using FlowAccount.Application.DTOs.Common;
using FlowAccount.Domain.Entities;
using FlowAccount.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FlowAccount.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Stock Management - Adjust stock levels and query inventory")]
public class StockController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StockController> _logger;

    public StockController(
        IUnitOfWork unitOfWork,
        ILogger<StockController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get stock information for a specific item
    /// </summary>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="itemType">Item type (Product or Variant)</param>
    /// <param name="itemId">Item ID</param>
    /// <returns>Stock information</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get stock information",
        Description = "Retrieves stock level for a specific item in a warehouse",
        OperationId = "GetStock"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Stock retrieved successfully", typeof(ResponseDto<StockDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Stock not found", typeof(ProblemDetails))]
    [ProducesResponseType(typeof(ResponseDto<StockDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<StockDto>>> GetStock(
        [FromQuery] int warehouseId,
        [FromQuery] string itemType,
        [FromQuery] int itemId)
    {
        try
        {
            var availableQuantity = await _unitOfWork.Stocks.GetAvailableQuantityAsync(
                warehouseId,
                itemType,
                itemId);

            return Ok(new ResponseDto<StockDto>
            {
                Success = true,
                Message = "Stock retrieved successfully",
                Data = new StockDto
                {
                    WarehouseId = warehouseId,
                    ItemType = itemType,
                    ItemId = itemId,
                    AvailableQuantity = availableQuantity,
                    LastUpdated = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error retrieving stock: WarehouseId={WarehouseId}, ItemType={ItemType}, ItemId={ItemId}",
                warehouseId, itemType, itemId);
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving stock information",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            );
        }
    }

    /// <summary>
    /// Adjust stock level for an item
    /// </summary>
    /// <param name="request">Stock adjustment details</param>
    /// <returns>Updated stock information</returns>
    [HttpPost("adjust")]
    [SwaggerOperation(
        Summary = "Adjust stock level",
        Description = "Adjusts stock quantity for a specific item in a warehouse",
        OperationId = "AdjustStock"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Stock adjusted successfully", typeof(ResponseDto<StockDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data", typeof(ProblemDetails))]
    [ProducesResponseType(typeof(ResponseDto<StockDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseDto<StockDto>>> AdjustStock(
        [FromBody] AdjustStockRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Problem(
                    title: "Validation Error",
                    detail: "The request contains invalid data",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
            }

            // Get current stock
            var currentQuantity = await _unitOfWork.Stocks.GetAvailableQuantityAsync(
                request.WarehouseId,
                request.ItemType,
                request.ItemId);

            // Calculate new quantity
            var newQuantity = request.AdjustmentType.ToLower() switch
            {
                "stockin" => currentQuantity + request.Quantity,
                "stockout" => currentQuantity - request.Quantity,
                "set" => request.Quantity,
                _ => throw new ArgumentException($"Invalid adjustment type: {request.AdjustmentType}")
            };

            if (newQuantity < 0)
            {
                return Problem(
                    title: "Insufficient Stock",
                    detail: $"Cannot reduce stock below zero. Current: {currentQuantity}, Requested: {request.Quantity}",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
            }

            // Update stock
            await _unitOfWork.Stocks.UpdateStockQuantityAsync(
                request.WarehouseId,
                request.ItemType,
                request.ItemId,
                newQuantity);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Stock adjusted: WarehouseId={WarehouseId}, ItemType={ItemType}, ItemId={ItemId}, " +
                "From={From}, To={To}, Type={Type}, Reason={Reason}",
                request.WarehouseId, request.ItemType, request.ItemId,
                currentQuantity, newQuantity, request.AdjustmentType, request.Reason);

            return Ok(new ResponseDto<StockDto>
            {
                Success = true,
                Message = "Stock adjusted successfully",
                Data = new StockDto
                {
                    WarehouseId = request.WarehouseId,
                    ItemType = request.ItemType,
                    ItemId = request.ItemId,
                    AvailableQuantity = newQuantity,
                    LastUpdated = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adjusting stock");
            return Problem(
                title: "Internal Server Error",
                detail: "An error occurred while adjusting stock",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext.Request.Path
            );
        }
    }
}

public class AdjustStockRequest
{
    public int WarehouseId { get; set; }
    public string ItemType { get; set; } = "Variant"; // "Product" or "Variant"
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string AdjustmentType { get; set; } = "StockIn"; // "StockIn", "StockOut", "Set"
    public string? Reason { get; set; }
}

public class StockDto
{
    public int WarehouseId { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTime LastUpdated { get; set; }
}
