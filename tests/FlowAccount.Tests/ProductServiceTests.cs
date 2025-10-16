using AutoMapper;
using FlowAccount.Application.DTOs.Product;
using FlowAccount.Application.Services;
using FlowAccount.Domain.Entities;
using FlowAccount.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlowAccount.Tests;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IVariantRepository> _mockVariantRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockVariantRepo = new Mock<IVariantRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<ProductService>>();

        // Setup UnitOfWork to return mocked repositories
        _mockUnitOfWork.Setup(u => u.Products).Returns(_mockProductRepo.Object);
        _mockUnitOfWork.Setup(u => u.Variants).Returns(_mockVariantRepo.Object);

        _productService = new ProductService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task GetProductByIdAsync_WithExistingId_ReturnsProduct()
    {
        // Arrange
        var productId = 1;
        var productEntity = new ProductMaster
        {
            Id = productId,
            Name = "T-Shirt",
            IsActive = true
        };

        var productDto = new ProductDto
        {
            Id = productId,
            Name = "T-Shirt"
        };

        // Setup for GetProductWithVariantsAsync (actual method called by service)
        _mockProductRepo.Setup(r => r.GetProductWithVariantsAsync(productId))
            .ReturnsAsync(productEntity);
        _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<ProductMaster>()))
            .Returns(productDto);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be("T-Shirt");
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var productId = 999;

        // Setup for GetProductWithVariantsAsync (actual method called by service)
        _mockProductRepo.Setup(r => r.GetProductWithVariantsAsync(productId))
            .ReturnsAsync((ProductMaster?)null);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<ProductMaster>
        {
            new ProductMaster { Id = 1, Name = "T-Shirt", IsActive = true },
            new ProductMaster { Id = 2, Name = "Pants", IsActive = true }
        };

        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "T-Shirt" },
            new ProductDto { Id = 2, Name = "Pants" }
        };

        _mockProductRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);
        _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<ProductMaster>>()))
            .Returns(productDtos);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("T-Shirt");
        result.Last().Name.Should().Be("Pants");
    }

    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        // Arrange & Act
        var service = new ProductService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );

        // Assert
        service.Should().NotBeNull();
    }
}
