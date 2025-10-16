using AutoMapper;
using FlowAccount.Application.DTOs.Bundle;
using FlowAccount.Application.Services;
using FlowAccount.Domain.Entities;
using FlowAccount.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlowAccount.Tests;

public class BundleServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IBundleRepository> _mockBundleRepo;
    private readonly Mock<IStockRepository> _mockStockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<BundleService>> _mockLogger;
    private readonly BundleService _bundleService;

    public BundleServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockBundleRepo = new Mock<IBundleRepository>();
        _mockStockRepo = new Mock<IStockRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<BundleService>>();

        // Setup UnitOfWork
        _mockUnitOfWork.Setup(u => u.Bundles).Returns(_mockBundleRepo.Object);
        _mockUnitOfWork.Setup(u => u.Stocks).Returns(_mockStockRepo.Object);

        _bundleService = new BundleService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task GetBundleByIdAsync_WithExistingId_ReturnsBundle()
    {
        // Arrange
        var bundleId = 1;
        var bundleEntity = new Bundle
        {
            Id = bundleId,
            Name = "Premium Bundle",
            Price = 499.00m
        };

        var bundleDto = new BundleDto
        {
            Id = bundleId,
            Name = "Premium Bundle",
            Price = 499.00m
        };

        // Setup for GetBundleWithItemsAsync (actual method called by service)
        _mockBundleRepo.Setup(r => r.GetBundleWithItemsAsync(bundleId))
            .ReturnsAsync(bundleEntity);
        _mockMapper.Setup(m => m.Map<BundleDto>(It.IsAny<Bundle>()))
            .Returns(bundleDto);

        // Act
        var result = await _bundleService.GetBundleByIdAsync(bundleId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(bundleId);
        result.Name.Should().Be("Premium Bundle");
    }

    [Fact]
    public async Task GetBundleByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var bundleId = 999;

        // Setup for GetBundleWithItemsAsync (actual method called by service)
        _mockBundleRepo.Setup(r => r.GetBundleWithItemsAsync(bundleId))
            .ReturnsAsync((Bundle?)null);

        // Act
        var result = await _bundleService.GetBundleByIdAsync(bundleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllBundlesAsync_ReturnsAllBundles()
    {
        // Arrange
        var bundles = new List<Bundle>
        {
            new Bundle { Id = 1, Name = "Bundle 1", Price = 499.00m },
            new Bundle { Id = 2, Name = "Bundle 2", Price = 599.00m }
        };

        var bundleDtos = new List<BundleDto>
        {
            new BundleDto { Id = 1, Name = "Bundle 1", Price = 499.00m },
            new BundleDto { Id = 2, Name = "Bundle 2", Price = 599.00m }
        };

        _mockBundleRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(bundles);
        _mockMapper.Setup(m => m.Map<IEnumerable<BundleDto>>(It.IsAny<IEnumerable<Bundle>>()))
            .Returns(bundleDtos);

        // Act
        var result = await _bundleService.GetAllBundlesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Bundle 1");
    }

    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        // Arrange & Act
        var service = new BundleService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );

        // Assert
        service.Should().NotBeNull();
    }
}
