using FlowAccount.Application.DTOs.Common;
using FlowAccount.Application.DTOs.Product;
using FlowAccount.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlowAccount.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get all products with their variants
    /// </summary>
    /// <returns>List of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ResponseDto<List<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto<List<ProductDto>>>> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(new ResponseDto<List<ProductDto>>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = products.ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, new ResponseDto<List<ProductDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving products",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDto<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<ProductDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<ProductDto>>> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new ResponseDto<ProductDto>
                {
                    Success = false,
                    Message = $"Product with ID {id} not found"
                });
            }

            return Ok(new ResponseDto<ProductDto>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = product
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, new ResponseDto<ProductDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the product",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Create a new product with variant options
    /// </summary>
    /// <param name="request">Product creation details</param>
    /// <returns>Created product</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/products
    ///     {
    ///         "name": "T-Shirt",
    ///         "sku": "TS-001",
    ///         "categoryId": 1,
    ///         "isActive": true,
    ///         "variantOptions": [
    ///             {
    ///                 "name": "Size",
    ///                 "displayOrder": 1,
    ///                 "values": ["S", "M", "L", "XL"]
    ///             },
    ///             {
    ///                 "name": "Color",
    ///                 "displayOrder": 2,
    ///                 "values": ["Red", "Blue", "Green"]
    ///             }
    ///         ]
    ///     }
    /// 
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseDto<ProductDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseDto<ProductDto>>> CreateProduct(
        [FromBody] CreateProductRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<ProductDto>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var product = await _productService.CreateProductAsync(request);
            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                new ResponseDto<ProductDto>
                {
                    Success = true,
                    Message = "Product created successfully",
                    Data = product
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, new ResponseDto<ProductDto>
            {
                Success = false,
                Message = "An error occurred while creating the product",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Updated product details</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResponseDto<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<ProductDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<ProductDto>>> UpdateProduct(
        int id,
        [FromBody] CreateProductRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<ProductDto>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var product = await _productService.UpdateProductAsync(id, request);
            if (product == null)
            {
                return NotFound(new ResponseDto<ProductDto>
                {
                    Success = false,
                    Message = $"Product with ID {id} not found"
                });
            }

            return Ok(new ResponseDto<ProductDto>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = product
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, new ResponseDto<ProductDto>
            {
                Success = false,
                Message = "An error occurred while updating the product",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto<object>>> DeleteProduct(int id)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound(new ResponseDto<object>
                {
                    Success = false,
                    Message = $"Product with ID {id} not found"
                });
            }

            return Ok(new ResponseDto<object>
            {
                Success = true,
                Message = "Product deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, new ResponseDto<object>
            {
                Success = false,
                Message = "An error occurred while deleting the product",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Generate product variants based on selected options (BATCH OPERATION - up to 250 variants)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Variant generation configuration</param>
    /// <returns>Generation result with total variants created and processing time</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/products/5/generate-variants
    ///     {
    ///         "productMasterId": 5,
    ///         "selectedOptions": {
    ///             "1": [1, 2, 3, 4],     // Size option: S, M, L, XL (option IDs)
    ///             "2": [5, 6, 7]         // Color option: Red, Blue, Green (option IDs)
    ///         },
    ///         "priceStrategy": "SizeAdjusted",
    ///         "basePrice": 299.00,
    ///         "baseCost": 150.00,
    ///         "skuPattern": "{ProductSKU}-{Size}-{Color}"
    ///     }
    ///     
    /// This will generate: 4 sizes × 3 colors = 12 variants
    /// SKU example: TS-001-M-Red, TS-001-M-Blue, etc.
    /// 
    /// Price strategies:
    /// - Fixed: All variants same price
    /// - SizeAdjusted: S=base, M=+20, L=+40, XL=+60
    /// - ColorAdjusted: (custom adjustments per color)
    /// 
    /// </remarks>
    [HttpPost("{id}/generate-variants")]
    [ProducesResponseType(typeof(ResponseDto<GenerateVariantsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto<GenerateVariantsResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseDto<GenerateVariantsResponse>>> GenerateVariants(
        int id,
        [FromBody] GenerateVariantsRequest request)
    {
        try
        {
            if (id != request.ProductMasterId)
            {
                return BadRequest(new ResponseDto<GenerateVariantsResponse>
                {
                    Success = false,
                    Message = "Product ID mismatch"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<GenerateVariantsResponse>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var response = await _productService.GenerateVariantsAsync(request);
            return Ok(new ResponseDto<GenerateVariantsResponse>
            {
                Success = true,
                Message = $"Successfully generated {response.TotalVariantsGenerated} variants in {response.ProcessingTime.TotalMilliseconds:F2}ms",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid variant generation request for product {ProductId}", id);
            return BadRequest(new ResponseDto<GenerateVariantsResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating variants for product {ProductId}", id);
            return StatusCode(500, new ResponseDto<GenerateVariantsResponse>
            {
                Success = false,
                Message = "An error occurred while generating variants",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}
