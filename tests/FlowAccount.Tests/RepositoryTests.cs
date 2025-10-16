using FlowAccount.Domain.Entities;
using FlowAccount.Infrastructure.Data;
using FlowAccount.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlowAccount.Tests;

public class RepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task ProductRepository_AddAsync_AddsProductSuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ProductRepository(context);
        var product = new ProductMaster
        {
            Name = "Test Product",
            Description = "Test Description",
            IsActive = true
        };

        // Act
        await repository.AddAsync(product);
        await context.SaveChangesAsync();

        // Assert
        var savedProduct = await context.ProductMasters.FirstOrDefaultAsync();
        savedProduct.Should().NotBeNull();
        savedProduct!.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task ProductRepository_GetByIdAsync_ReturnsCorrectProduct()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ProductRepository(context);

        var product = new ProductMaster
        {
            Name = "Test Product"
        };

        await context.ProductMasters.AddAsync(product);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task ProductRepository_Update_UpdatesProductSuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ProductRepository(context);

        var product = new ProductMaster
        {
            Name = "Original Name"
        };

        await context.ProductMasters.AddAsync(product);
        await context.SaveChangesAsync();

        // Act
        product.Name = "Updated Name";
        repository.Update(product);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.ProductMasters.FindAsync(product.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task ProductRepository_GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ProductRepository(context);

        await context.ProductMasters.AddRangeAsync(
            new ProductMaster { Name = "Product 1" },
            new ProductMaster { Name = "Product 2" },
            new ProductMaster { Name = "Product 3" }
        );
        await context.SaveChangesAsync();

        // Act
        var results = await repository.GetAllAsync();

        // Assert
        results.Should().HaveCount(3);
        results.Select(p => p.Name).Should().Contain(new[] { "Product 1", "Product 2", "Product 3" });
    }

    [Fact]
    public async Task BundleRepository_AddAsync_AddsBundleSuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new BundleRepository(context);

        var bundle = new Bundle
        {
            Name = "Test Bundle",
            Price = 499.00m
        };

        // Act
        await repository.AddAsync(bundle);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Bundles.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Test Bundle");
        saved.Price.Should().Be(499.00m);
    }

    [Fact]
    public async Task StockRepository_AddAsync_AddsStockSuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new StockRepository(context);

        var stock = new Stock
        {
            ItemType = "Variant",
            ItemId = 1,
            WarehouseId = 1,
            Quantity = 100
        };

        // Act
        await repository.AddAsync(stock);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Stocks.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Quantity.Should().Be(100);
        saved.ItemType.Should().Be("Variant");
    }
    [Fact]
    public async Task VariantRepository_AddAsync_AddsVariantSuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new VariantRepository(context);

        var variant = new ProductVariant
        {
            ProductMasterId = 1,
            Price = 299.00m
        };

        // Act
        await repository.AddAsync(variant);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.ProductVariants.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Price.Should().Be(299.00m);
    }

    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_PersistsChanges()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var unitOfWork = new UnitOfWork(context);

        var product = new ProductMaster
        {
            Name = "Test Product"
        };

        await unitOfWork.Products.AddAsync(product);

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().BeGreaterThan(0);
        var saved = await context.ProductMasters.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Test Product");
    }

    [Fact(Skip = "InMemory database does not support transactions")]
    public async Task UnitOfWork_Transaction_CommitsSuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var unitOfWork = new UnitOfWork(context);

        // Act
        await unitOfWork.BeginTransactionAsync();

        var product = new ProductMaster { Name = "Product in Transaction" };
        await unitOfWork.Products.AddAsync(product);
        await context.SaveChangesAsync();

        await unitOfWork.CommitTransactionAsync();

        // Assert
        var saved = await context.ProductMasters.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Product in Transaction");
    }
}
