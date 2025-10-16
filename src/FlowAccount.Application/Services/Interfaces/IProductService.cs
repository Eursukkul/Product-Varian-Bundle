using FlowAccount.Application.DTOs.Product;

namespace FlowAccount.Application.Services.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto> UpdateProductAsync(int id, CreateProductRequest request);
    Task<bool> DeleteProductAsync(int id);

    /// <summary>
    /// Generate multiple variants for a product (BATCH OPERATION)
    /// Supports creating up to 250 variants at once
    /// </summary>
    Task<GenerateVariantsResponse> GenerateVariantsAsync(GenerateVariantsRequest request);
}
