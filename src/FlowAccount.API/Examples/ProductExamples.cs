using FlowAccount.Application.DTOs.Common;
using FlowAccount.Application.DTOs.Product;
using Swashbuckle.AspNetCore.Filters;

namespace FlowAccount.API.Examples;

/// <summary>
/// Example for CreateProduct request
/// </summary>
public class CreateProductRequestExample : IExamplesProvider<CreateProductRequest>
{
    public CreateProductRequest GetExamples()
    {
        return new CreateProductRequest
        {
            Name = "Premium T-Shirt Collection",
            Description = "High-quality cotton t-shirts with multiple size and color options",
            CategoryId = 1,
            IsActive = true,
            VariantOptions = new List<CreateVariantOptionRequest>
            {
                new CreateVariantOptionRequest
                {
                    Name = "Size",
                    DisplayOrder = 1,
                    Values = new List<string> { "XS", "S", "M", "L", "XL", "XXL" }
                },
                new CreateVariantOptionRequest
                {
                    Name = "Color",
                    DisplayOrder = 2,
                    Values = new List<string> { "White", "Black", "Navy", "Gray" }
                },
                new CreateVariantOptionRequest
                {
                    Name = "Material",
                    DisplayOrder = 3,
                    Values = new List<string> { "100% Cotton", "Cotton Blend" }
                }
            }
        };
    }
}

/// <summary>
/// Example for ProductList response
/// </summary>
public class ProductListResponseExample : IExamplesProvider<ResponseDto<List<ProductDto>>>
{
    public ResponseDto<List<ProductDto>> GetExamples()
    {
        return new ResponseDto<List<ProductDto>>
        {
            Success = true,
            Message = "Products retrieved successfully",
            Data = new List<ProductDto>
            {
                new ProductDto
                {
                    Id = 10,
                    Name = "Ultimate T-Shirt Collection",
                    Description = "Premium quality t-shirts with various size, color, and material combinations",
                    CategoryId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    VariantOptions = new List<VariantOptionDto>
                    {
                        new VariantOptionDto
                        {
                            Id = 17,
                            Name = "Size",
                            DisplayOrder = 1,
                            Values = new List<VariantOptionValueDto>
                            {
                                new VariantOptionValueDto { Id = 82, Value = "XS" },
                                new VariantOptionValueDto { Id = 83, Value = "S" },
                                new VariantOptionValueDto { Id = 84, Value = "M" },
                                new VariantOptionValueDto { Id = 85, Value = "L" },
                                new VariantOptionValueDto { Id = 86, Value = "XL" }
                            }
                        },
                        new VariantOptionDto
                        {
                            Id = 18,
                            Name = "Color",
                            DisplayOrder = 2,
                            Values = new List<VariantOptionValueDto>
                            {
                                new VariantOptionValueDto { Id = 92, Value = "Black" },
                                new VariantOptionValueDto { Id = 93, Value = "White" },
                                new VariantOptionValueDto { Id = 94, Value = "Red" }
                            }
                        }
                    },
                    ProductVariants = new List<ProductVariantDto>
                    {
                        new ProductVariantDto
                        {
                            Id = 101,
                            SKU = "ULTIMATE-M-BLACK",
                            Price = 299.00m,
                            Cost = 150.00m,
                            IsActive = true
                        },
                        new ProductVariantDto
                        {
                            Id = 102,
                            SKU = "ULTIMATE-L-WHITE",
                            Price = 319.00m,
                            Cost = 150.00m,
                            IsActive = true
                        }
                    }
                },
                new ProductDto
                {
                    Id = 11,
                    Name = "Sports Shorts",
                    Description = "Athletic shorts for active lifestyle",
                    CategoryId = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    VariantOptions = new List<VariantOptionDto>
                    {
                        new VariantOptionDto
                        {
                            Id = 19,
                            Name = "Size",
                            DisplayOrder = 1,
                            Values = new List<VariantOptionValueDto>
                            {
                                new VariantOptionValueDto { Id = 95, Value = "S" },
                                new VariantOptionValueDto { Id = 96, Value = "M" },
                                new VariantOptionValueDto { Id = 97, Value = "L" }
                            }
                        }
                    },
                    ProductVariants = new List<ProductVariantDto>
                    {
                        new ProductVariantDto
                        {
                            Id = 201,
                            SKU = "SHORTS-M",
                            Price = 399.00m,
                            Cost = 200.00m,
                            IsActive = true
                        }
                    }
                }
            }
        };
    }
}

/// <summary>
/// Example for single Product response
/// </summary>
public class ProductResponseExample : IExamplesProvider<ResponseDto<ProductDto>>
{
    public ResponseDto<ProductDto> GetExamples()
    {
        return new ResponseDto<ProductDto>
        {
            Success = true,
            Message = "Product retrieved successfully",
            Data = new ProductDto
            {
                Id = 10,
                Name = "Ultimate T-Shirt Collection",
                Description = "Premium quality t-shirts with various size, color, and material combinations. Perfect for everyday wear or sports activities.",
                CategoryId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-5),
                VariantOptions = new List<VariantOptionDto>
                {
                    new VariantOptionDto
                    {
                        Id = 17,
                        Name = "Size",
                        DisplayOrder = 1,
                        Values = new List<VariantOptionValueDto>
                        {
                            new VariantOptionValueDto { Id = 82, Value = "XS" },
                            new VariantOptionValueDto { Id = 83, Value = "S" },
                            new VariantOptionValueDto { Id = 84, Value = "M" },
                            new VariantOptionValueDto { Id = 85, Value = "L" },
                            new VariantOptionValueDto { Id = 86, Value = "XL" }
                        }
                    },
                    new VariantOptionDto
                    {
                        Id = 18,
                        Name = "Color",
                        DisplayOrder = 2,
                        Values = new List<VariantOptionValueDto>
                        {
                            new VariantOptionValueDto { Id = 92, Value = "Black" },
                            new VariantOptionValueDto { Id = 93, Value = "White" },
                            new VariantOptionValueDto { Id = 94, Value = "Red" }
                        }
                    },
                    new VariantOptionDto
                    {
                        Id = 19,
                        Name = "Material",
                        DisplayOrder = 3,
                        Values = new List<VariantOptionValueDto>
                        {
                            new VariantOptionValueDto { Id = 97, Value = "Cotton" },
                            new VariantOptionValueDto { Id = 98, Value = "Polyester" }
                        }
                    }
                },
                ProductVariants = new List<ProductVariantDto>
                {
                    new ProductVariantDto
                    {
                        Id = 101,
                        SKU = "ULTIMATE-M-BLACK-COTTON",
                        Price = 299.00m,
                        Cost = 150.00m,
                        IsActive = true,
                        Attributes = new List<VariantAttributeDto>
                        {
                            new VariantAttributeDto { OptionName = "Size", OptionValue = "M" },
                            new VariantAttributeDto { OptionName = "Color", OptionValue = "Black" },
                            new VariantAttributeDto { OptionName = "Material", OptionValue = "Cotton" }
                        }
                    },
                    new ProductVariantDto
                    {
                        Id = 102,
                        SKU = "ULTIMATE-L-WHITE-COTTON",
                        Price = 319.00m,
                        Cost = 150.00m,
                        IsActive = true,
                        Attributes = new List<VariantAttributeDto>
                        {
                            new VariantAttributeDto { OptionName = "Size", OptionValue = "L" },
                            new VariantAttributeDto { OptionName = "Color", OptionValue = "White" },
                            new VariantAttributeDto { OptionName = "Material", OptionValue = "Cotton" }
                        }
                    },
                    new ProductVariantDto
                    {
                        Id = 103,
                        SKU = "ULTIMATE-XL-RED-POLYESTER",
                        Price = 339.00m,
                        Cost = 150.00m,
                        IsActive = true,
                        Attributes = new List<VariantAttributeDto>
                        {
                            new VariantAttributeDto { OptionName = "Size", OptionValue = "XL" },
                            new VariantAttributeDto { OptionName = "Color", OptionValue = "Red" },
                            new VariantAttributeDto { OptionName = "Material", OptionValue = "Polyester" }
                        }
                    }
                }
            }
        };
    }
}

/// <summary>
/// Example for Update Product request (same as Create)
/// </summary>
public class UpdateProductRequestExample : IExamplesProvider<CreateProductRequest>
{
    public CreateProductRequest GetExamples()
    {
        return new CreateProductRequest
        {
            Name = "Premium T-Shirt Collection (Updated)",
            Description = "Updated description: High-quality cotton t-shirts with enhanced comfort and durability",
            CategoryId = 1,
            IsActive = true,
            VariantOptions = new List<CreateVariantOptionRequest>
            {
                new CreateVariantOptionRequest
                {
                    Name = "Size",
                    DisplayOrder = 1,
                    Values = new List<string> { "XS", "S", "M", "L", "XL", "XXL", "XXXL" } // Added XXXL
                },
                new CreateVariantOptionRequest
                {
                    Name = "Color",
                    DisplayOrder = 2,
                    Values = new List<string> { "White", "Black", "Navy", "Gray", "Olive" } // Added Olive
                },
                new CreateVariantOptionRequest
                {
                    Name = "Material",
                    DisplayOrder = 3,
                    Values = new List<string> { "100% Cotton", "Cotton Blend", "Organic Cotton" } // Added Organic
                }
            }
        };
    }
}
