using AutoMapper;
using FlowAccount.Application.DTOs.Product;
using FlowAccount.Application.Services.Interfaces;
using FlowAccount.Domain.Entities;
using FlowAccount.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FlowAccount.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetProductWithVariantsAsync(id);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetActiveProductsAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        _logger.LogInformation(
            "Creating new product: Name={ProductName}, CategoryId={CategoryId}, VariantOptions={OptionCount}",
            request.Name,
            request.CategoryId,
            request.VariantOptions.Count);

        try
        {
            // Create Product Master
            var product = new ProductMaster
            {
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogDebug("Product master created: ProductId={ProductId}", product.Id);

            // Create Variant Options and Values
            var variantOptions = new List<VariantOption>();

            foreach (var optionRequest in request.VariantOptions)
            {
                var option = new VariantOption
                {
                    ProductMasterId = product.Id,
                    Name = optionRequest.Name,
                    DisplayOrder = optionRequest.DisplayOrder,
                    Values = new List<VariantOptionValue>()
                };

                // Create Option Values
                for (int i = 0; i < optionRequest.Values.Count; i++)
                {
                    var value = new VariantOptionValue
                    {
                        VariantOptionId = option.Id,
                        Value = optionRequest.Values[i],
                        DisplayOrder = i
                    };

                    option.Values.Add(value);
                }

                variantOptions.Add(option);

                _logger.LogDebug(
                    "Added variant option: Name={OptionName}, Values={ValueCount}",
                    option.Name,
                    option.Values.Count);
            }

            // Add options with values
            product.VariantOptions = variantOptions;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully created product: ProductId={ProductId}, Name={ProductName}, OptionCount={OptionCount}",
                product.Id,
                product.Name,
                variantOptions.Count);

            // Return created product with all details
            var createdProduct = await _unitOfWork.Products.GetProductWithOptionsAsync(product.Id);
            return _mapper.Map<ProductDto>(createdProduct!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create product: Name={ProductName}, CategoryId={CategoryId}",
                request.Name,
                request.CategoryId);
            throw;
        }
    }

    public async Task<ProductDto> UpdateProductAsync(int id, CreateProductRequest request)
    {
        _logger.LogInformation(
            "Updating product: ProductId={ProductId}, Name={ProductName}",
            id,
            request.Name);

        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for update: ProductId={ProductId}", id);
                throw new KeyNotFoundException($"Product with ID {id} not found");
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.CategoryId = request.CategoryId;
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully updated product: ProductId={ProductId}, Name={ProductName}",
                id,
                product.Name);

            var updatedProduct = await _unitOfWork.Products.GetProductWithVariantsAsync(id);
            return _mapper.Map<ProductDto>(updatedProduct!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to update product: ProductId={ProductId}, Name={ProductName}",
                id,
                request.Name);
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        _logger.LogInformation("Deleting product: ProductId={ProductId}", id);

        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for deletion: ProductId={ProductId}", id);
                return false;
            }

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted product: ProductId={ProductId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete product: ProductId={ProductId}", id);
            throw;
        }
    }

    /// <summary>
    /// BATCH OPERATION: Generate multiple variants (up to 250)
    /// This implements the complex business logic for creating variants with combinations
    /// </summary>
    public async Task<GenerateVariantsResponse> GenerateVariantsAsync(GenerateVariantsRequest request)
    {
        _logger.LogInformation(
            "Starting variant generation: ProductId={ProductId}, SelectedOptions={OptionCount}, BasePrice={BasePrice}",
            request.ProductMasterId,
            request.SelectedOptions.Count,
            request.BasePrice);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Validate product exists
            var product = await _unitOfWork.Products.GetProductWithOptionsAsync(request.ProductMasterId);
            if (product == null)
            {
                _logger.LogWarning(
                    "Product not found for variant generation: ProductId={ProductId}",
                    request.ProductMasterId);
                throw new KeyNotFoundException($"Product with ID {request.ProductMasterId} not found");
            }

            // Get all selected option values
            var allOptionValues = new List<List<VariantOptionValue>>();

            foreach (var (optionId, valueIds) in request.SelectedOptions)
            {
                var option = product.VariantOptions.FirstOrDefault(o => o.Id == optionId);
                if (option == null) continue;

                var selectedValues = option.Values.Where(v => valueIds.Contains(v.Id)).ToList();
                allOptionValues.Add(selectedValues);

                _logger.LogDebug(
                    "Selected option: OptionId={OptionId}, OptionName={OptionName}, ValueCount={ValueCount}",
                    optionId,
                    option.Name,
                    selectedValues.Count);
            }

            // Generate all combinations (Cartesian product)
            var combinations = GetCombinations(allOptionValues);

            _logger.LogInformation(
                "Generated Cartesian product: ProductId={ProductId}, CombinationCount={CombinationCount}",
                request.ProductMasterId,
                combinations.Count);

            // Validate we're not exceeding 500 variants
            if (combinations.Count > 500)
            {
                _logger.LogError(
                    "Variant generation limit exceeded: ProductId={ProductId}, RequestedCount={RequestedCount}, MaxAllowed=500",
                    request.ProductMasterId,
                    combinations.Count);
                throw new InvalidOperationException($"Cannot generate more than 500 variants. Requested: {combinations.Count}");
            }

            var generatedVariants = new List<ProductVariant>();

            await _unitOfWork.BeginTransactionAsync();
            _logger.LogDebug("Transaction started for variant generation");

            foreach (var combination in combinations)
            {
                // Generate SKU
                var sku = GenerateSKU(product.Name, combination, request.SkuPattern);

                // Calculate price based on strategy
                var price = CalculatePrice(request.BasePrice, combination, request.PriceStrategy);

                // Create variant
                var variant = new ProductVariant
                {
                    ProductMasterId = product.Id,
                    SKU = sku,
                    Price = price,
                    Cost = request.BaseCost,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Variants.AddAsync(variant);
                await _unitOfWork.SaveChangesAsync();

                // Create variant attributes
                variant.Attributes = combination.Select(optionValue => new ProductVariantAttribute
                {
                    ProductVariantId = variant.Id,
                    VariantOptionValueId = optionValue.Id
                }).ToList();

                _unitOfWork.Variants.Update(variant);

                generatedVariants.Add(variant);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            stopwatch.Stop();

            _logger.LogInformation(
                "Variant generation completed successfully: ProductId={ProductId}, VariantsGenerated={VariantCount}, Duration={DurationMs}ms, PriceStrategy={PriceStrategy}",
                product.Id,
                generatedVariants.Count,
                stopwatch.ElapsedMilliseconds,
                request.PriceStrategy);

            // Return response
            return new GenerateVariantsResponse
            {
                ProductMasterId = product.Id,
                ProductName = product.Name,
                TotalVariantsGenerated = generatedVariants.Count,
                Variants = _mapper.Map<List<ProductVariantDto>>(generatedVariants),
                ProcessingTime = stopwatch.Elapsed,
                Message = $"Successfully generated {generatedVariants.Count} variants in {stopwatch.ElapsedMilliseconds}ms"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Variant generation failed: ProductId={ProductId}, OptionCount={OptionCount}, Duration={DurationMs}ms",
                request.ProductMasterId,
                request.SelectedOptions.Count,
                stopwatch.ElapsedMilliseconds);

            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogWarning("Transaction rolled back for variant generation");
            throw;
        }
    }

    #region Helper Methods

    private List<List<VariantOptionValue>> GetCombinations(List<List<VariantOptionValue>> lists)
    {
        if (!lists.Any()) return new List<List<VariantOptionValue>>();
        if (lists.Count == 1) return lists[0].Select(v => new List<VariantOptionValue> { v }).ToList();

        var result = new List<List<VariantOptionValue>>();
        var remainingCombinations = GetCombinations(lists.Skip(1).ToList());

        foreach (var value in lists[0])
        {
            foreach (var combination in remainingCombinations)
            {
                var newCombination = new List<VariantOptionValue> { value };
                newCombination.AddRange(combination);
                result.Add(newCombination);
            }
        }

        return result;
    }

    private string GenerateSKU(string productName, List<VariantOptionValue> combination, string? pattern)
    {
        if (!string.IsNullOrEmpty(pattern))
        {
            var sku = pattern;
            foreach (var value in combination)
            {
                sku = sku.Replace($"{{{value.VariantOption.Name}}}", value.Value.ToUpper());
            }
            return sku;
        }

        // Default SKU format: PRODUCTNAME-VALUE1-VALUE2
        var skuParts = new List<string> { productName.Replace(" ", "").ToUpper() };
        skuParts.AddRange(combination.Select(v => v.Value.ToUpper()));
        return string.Join("-", skuParts);
    }

    private decimal CalculatePrice(decimal basePrice, List<VariantOptionValue> combination, PriceStrategy strategy)
    {
        if (strategy == PriceStrategy.Fixed)
        {
            return basePrice;
        }

        // Add price adjustments based on attributes
        var adjustedPrice = basePrice;

        if (strategy == PriceStrategy.SizeAdjusted)
        {
            var sizeValue = combination.FirstOrDefault(v => v.VariantOption.Name.ToLower() == "size");
            if (sizeValue != null)
            {
                adjustedPrice += sizeValue.Value.ToUpper() switch
                {
                    "S" => 0,
                    "M" => 10,
                    "L" => 20,
                    "XL" => 30,
                    _ => 0
                };
            }
        }

        return adjustedPrice;
    }

    #endregion
}
