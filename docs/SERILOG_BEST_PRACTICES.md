# Serilog Best Practices & Usage Guide

## 🎯 Serilog ควรใช้งานที่ไหนบ้าง

---

## 1. ✅ API Controllers (ใช้แน่นอน)

### ทำไมต้องใช้:
- ติดตาม user requests และ responses
- Debug ปัญหาที่เกิดจาก client
- Monitor performance ของแต่ละ endpoint
- Track business operations

### ตัวอย่าง: ProductsController

```csharp
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

    [HttpPost("{id}/generate-variants")]
    public async Task<ActionResult<ResponseDto<GenerateVariantsResponse>>> GenerateVariants(
        int id,
        [FromBody] GenerateVariantsRequest request)
    {
        // 1. Log เมื่อเริ่มต้น operation ที่สำคัญ
        _logger.LogInformation(
            "Starting variant generation for product {ProductId} with {OptionCount} options",
            id,
            request.SelectedOptions.Count);

        try
        {
            var response = await _productService.GenerateVariantsAsync(request);
            
            // 2. Log เมื่อสำเร็จ พร้อม metrics
            _logger.LogInformation(
                "Generated {VariantCount} variants for product {ProductId} in {ProcessingTime}ms",
                response.TotalVariantsGenerated,
                id,
                response.ProcessingTime.TotalMilliseconds);
            
            return Ok(new ResponseDto<GenerateVariantsResponse>
            {
                Success = true,
                Message = $"Successfully generated {response.TotalVariantsGenerated} variants",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            // 3. Log business validation errors เป็น Warning
            _logger.LogWarning(ex, 
                "Invalid variant generation request for product {ProductId}: {ErrorMessage}", 
                id,
                ex.Message);
            
            return BadRequest(new ResponseDto<GenerateVariantsResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            // 4. Log unexpected errors เป็น Error
            _logger.LogError(ex, 
                "Unexpected error generating variants for product {ProductId}", 
                id);
            
            return StatusCode(500, new ResponseDto<GenerateVariantsResponse>
            {
                Success = false,
                Message = "An error occurred while generating variants"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDto<ProductDto>>> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                // 5. Log not found scenarios
                _logger.LogWarning("Product {ProductId} not found", id);
                return NotFound(new ResponseDto<ProductDto>
                {
                    Success = false,
                    Message = $"Product with ID {id} not found"
                });
            }

            return Ok(new ResponseDto<ProductDto>
            {
                Success = true,
                Data = product
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, new ResponseDto<ProductDto>
            {
                Success = false,
                Message = "An error occurred"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto<object>>> DeleteProduct(int id)
    {
        try
        {
            // 6. Log critical operations (Delete, Update)
            _logger.LogInformation("Attempting to delete product {ProductId}", id);
            
            var result = await _productService.DeleteProductAsync(id);
            
            if (!result)
            {
                _logger.LogWarning("Cannot delete product {ProductId} - not found", id);
                return NotFound(new ResponseDto<object>
                {
                    Success = false,
                    Message = $"Product with ID {id} not found"
                });
            }

            _logger.LogInformation("Successfully deleted product {ProductId}", id);
            
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
                Message = "An error occurred while deleting the product"
            });
        }
    }
}
```

### ตัวอย่าง: BundlesController

```csharp
public class BundlesController : ControllerBase
{
    private readonly IBundleService _bundleService;
    private readonly ILogger<BundlesController> _logger;

    [HttpPost("{id}/sell")]
    public async Task<ActionResult<ResponseDto<SellBundleResponse>>> SellBundle(
        int id,
        [FromBody] SellBundleRequest request)
    {
        // Log transaction operations พร้อมรายละเอียด
        _logger.LogInformation(
            "Processing bundle sale: BundleId={BundleId}, Quantity={Quantity}, WarehouseId={WarehouseId}",
            id,
            request.Quantity,
            request.WarehouseId);

        try
        {
            var response = await _bundleService.SellBundleAsync(request);
            
            // Log successful transaction พร้อม transaction ID
            _logger.LogInformation(
                "Bundle sale completed: TransactionId={TransactionId}, BundleId={BundleId}, Quantity={Quantity}, RemainingStock={RemainingStock}",
                response.TransactionId,
                id,
                request.Quantity,
                response.RemainingBundleStock);
            
            return Ok(new ResponseDto<SellBundleResponse>
            {
                Success = true,
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            // Log insufficient stock scenarios
            _logger.LogWarning(ex,
                "Insufficient stock for bundle sale: BundleId={BundleId}, RequestedQuantity={Quantity}",
                id,
                request.Quantity);
            
            return BadRequest(new ResponseDto<SellBundleResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            // Log transaction failures - CRITICAL
            _logger.LogError(ex,
                "CRITICAL: Failed to complete bundle sale transaction: BundleId={BundleId}, Quantity={Quantity}",
                id,
                request.Quantity);
            
            return StatusCode(500, new ResponseDto<SellBundleResponse>
            {
                Success = false,
                Message = "Transaction failed"
            });
        }
    }

    [HttpPost("{id}/calculate-stock")]
    public async Task<ActionResult<ResponseDto<BundleStockCalculationResponse>>> CalculateBundleStock(
        int id,
        [FromBody] CalculateBundleStockRequest request)
    {
        _logger.LogDebug(
            "Calculating stock for bundle {BundleId} in warehouse {WarehouseId}",
            id,
            request.WarehouseId);

        try
        {
            var response = await _bundleService.CalculateBundleStockAsync(request);
            
            // Log stock calculation results พร้อม bottleneck info
            _logger.LogInformation(
                "Stock calculation complete: BundleId={BundleId}, MaxBundles={MaxBundles}, BottleneckItems={BottleneckCount}",
                id,
                response.MaxAvailableBundles,
                response.ItemsStockBreakdown.Count(x => x.IsBottleneck));
            
            return Ok(new ResponseDto<BundleStockCalculationResponse>
            {
                Success = true,
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating stock for bundle {BundleId}", id);
            return StatusCode(500, new ResponseDto<BundleStockCalculationResponse>
            {
                Success = false,
                Message = "An error occurred"
            });
        }
    }
}
```

---

## 2. ✅ Application Services (ใช้แน่นอน)

### ทำไมต้องใช้:
- Log business logic execution
- Track complex algorithms (Cartesian product, stock calculation)
- Debug data transformation issues
- Monitor service performance

### ตัวอย่าง: ProductService

```csharp
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

    public async Task<GenerateVariantsResponse> GenerateVariantsAsync(GenerateVariantsRequest request)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // 1. Log start พร้อม input parameters
            _logger.LogInformation(
                "Starting variant generation for product {ProductId} with {OptionCount} options, PriceStrategy={PriceStrategy}",
                request.ProductMasterId,
                request.SelectedOptions.Count,
                request.PriceStrategy);

            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductMasterId);
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for variant generation", request.ProductMasterId);
                throw new KeyNotFoundException($"Product with ID {request.ProductMasterId} not found");
            }

            // 2. Log business rule validation
            var combinations = GetCombinations(request.SelectedOptions);
            _logger.LogInformation(
                "Generated {CombinationCount} combinations for product {ProductId}",
                combinations.Count,
                request.ProductMasterId);

            if (combinations.Count > 250)
            {
                _logger.LogWarning(
                    "Variant count {Count} exceeds maximum 250 for product {ProductId}",
                    combinations.Count,
                    request.ProductMasterId);
                throw new InvalidOperationException($"Cannot generate more than 250 variants. Current count: {combinations.Count}");
            }

            // 3. Log database transaction start
            _logger.LogDebug("Beginning database transaction for variant generation");
            await _unitOfWork.BeginTransactionAsync();

            var variants = new List<ProductVariant>();
            foreach (var combination in combinations)
            {
                var variant = new ProductVariant
                {
                    ProductMasterId = request.ProductMasterId,
                    SKU = GenerateSKU(product, combination, request.SkuPattern),
                    Price = CalculatePrice(combination, request),
                    Cost = request.BaseCost,
                    IsActive = true
                };

                await _unitOfWork.Variants.AddAsync(variant);
                variants.Add(variant);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            stopwatch.Stop();

            // 4. Log success พร้อม performance metrics
            _logger.LogInformation(
                "Successfully generated {VariantCount} variants for product {ProductId} in {ElapsedMs}ms (Database transaction time included)",
                variants.Count,
                request.ProductMasterId,
                stopwatch.ElapsedMilliseconds);

            return new GenerateVariantsResponse
            {
                ProductMasterId = request.ProductMasterId,
                ProductName = product.Name,
                TotalVariantsGenerated = variants.Count,
                ProcessingTime = stopwatch.Elapsed,
                Variants = _mapper.Map<List<ProductVariantDto>>(variants),
                Message = $"Successfully generated {variants.Count} product variants"
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // 5. Log errors พร้อม rollback
            _logger.LogError(ex,
                "Failed to generate variants for product {ProductId} after {ElapsedMs}ms. Rolling back transaction.",
                request.ProductMasterId,
                stopwatch.ElapsedMilliseconds);

            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private List<Dictionary<int, int>> GetCombinations(Dictionary<int, List<int>> selectedOptions)
    {
        // Log algorithm execution
        _logger.LogDebug(
            "Computing Cartesian product for {OptionCount} options",
            selectedOptions.Count);

        var combinations = new List<Dictionary<int, int>>();
        // ... algorithm implementation
        
        _logger.LogDebug("Cartesian product computed: {CombinationCount} combinations", combinations.Count);
        return combinations;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        _logger.LogInformation(
            "Creating new product: Name={ProductName}, SKU={SKU}, CategoryId={CategoryId}",
            request.Name,
            request.Sku,
            request.CategoryId);

        try
        {
            var product = new ProductMaster
            {
                Name = request.Name,
                SKU = request.Sku,
                CategoryId = request.CategoryId,
                IsActive = request.IsActive
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully created product: ProductId={ProductId}, Name={ProductName}",
                product.Id,
                product.Name);

            return _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create product: Name={ProductName}, SKU={SKU}",
                request.Name,
                request.Sku);
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);

        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for deletion", id);
                return false;
            }

            // Check for dependent data
            var hasVariants = await _unitOfWork.Variants.GetVariantsByProductIdAsync(id);
            if (hasVariants.Any())
            {
                _logger.LogWarning(
                    "Cannot delete product {ProductId} - has {VariantCount} dependent variants",
                    id,
                    hasVariants.Count());
                throw new InvalidOperationException("Cannot delete product with existing variants");
            }

            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted product {ProductId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete product {ProductId}", id);
            throw;
        }
    }
}
```

### ตัวอย่าง: BundleService

```csharp
public class BundleService : IBundleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BundleService> _logger;

    public async Task<BundleStockCalculationResponse> CalculateBundleStockAsync(
        CalculateBundleStockRequest request)
    {
        _logger.LogDebug(
            "Calculating stock for bundle {BundleId} in warehouse {WarehouseId}",
            request.BundleId,
            request.WarehouseId);

        try
        {
            var bundle = await _unitOfWork.Bundles.GetBundleWithItemsAsync(request.BundleId);
            if (bundle == null)
            {
                _logger.LogWarning("Bundle {BundleId} not found", request.BundleId);
                throw new KeyNotFoundException($"Bundle with ID {request.BundleId} not found");
            }

            var itemsStockBreakdown = new List<BundleItemStockInfo>();
            int minPossibleBundles = int.MaxValue;

            foreach (var item in bundle.Items)
            {
                // Log stock query for each item
                _logger.LogDebug(
                    "Querying stock for item: Type={ItemType}, Id={ItemId}, Required={RequiredQty}",
                    item.ItemType,
                    item.ItemId,
                    item.Quantity);

                var stock = await GetStockForItem(item, request.WarehouseId);
                int possibleBundles = stock.Quantity / item.Quantity;

                itemsStockBreakdown.Add(new BundleItemStockInfo
                {
                    ItemName = await GetItemName(item),
                    ItemSku = await GetItemSKU(item),
                    RequiredQuantity = item.Quantity,
                    AvailableQuantity = stock.Quantity,
                    PossibleBundles = possibleBundles
                });

                if (possibleBundles < minPossibleBundles)
                {
                    minPossibleBundles = possibleBundles;
                }
            }

            // Mark bottleneck items
            foreach (var item in itemsStockBreakdown)
            {
                item.IsBottleneck = item.PossibleBundles == minPossibleBundles;
            }

            var bottleneckItems = itemsStockBreakdown.Where(x => x.IsBottleneck).ToList();
            
            // Log calculation results
            _logger.LogInformation(
                "Stock calculation complete: Bundle={BundleId}, MaxBundles={MaxBundles}, BottleneckItems={BottleneckCount}",
                request.BundleId,
                minPossibleBundles,
                bottleneckItems.Count);

            if (bottleneckItems.Any())
            {
                _logger.LogDebug(
                    "Bottleneck items: {BottleneckItems}",
                    string.Join(", ", bottleneckItems.Select(x => x.ItemName)));
            }

            return new BundleStockCalculationResponse
            {
                MaxAvailableBundles = minPossibleBundles,
                ItemsStockBreakdown = itemsStockBreakdown,
                WarehouseName = $"Warehouse {request.WarehouseId}",
                Explanation = bottleneckItems.Any()
                    ? $"Bundle availability limited by {bottleneckItems.First().ItemName} - only {bottleneckItems.First().AvailableQuantity} units available"
                    : "All items in stock"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to calculate stock for bundle {BundleId} in warehouse {WarehouseId}",
                request.BundleId,
                request.WarehouseId);
            throw;
        }
    }

    public async Task<SellBundleResponse> SellBundleAsync(SellBundleRequest request)
    {
        _logger.LogInformation(
            "Processing bundle sale: BundleId={BundleId}, Quantity={Quantity}, Warehouse={WarehouseId}, AllowBackorder={AllowBackorder}",
            request.BundleId,
            request.Quantity,
            request.WarehouseId,
            request.AllowBackorder);

        try
        {
            // Calculate available stock first
            var stockCalc = await CalculateBundleStockAsync(new CalculateBundleStockRequest
            {
                BundleId = request.BundleId,
                WarehouseId = request.WarehouseId
            });

            // Validate stock availability
            if (!request.AllowBackorder && stockCalc.MaxAvailableBundles < request.Quantity)
            {
                _logger.LogWarning(
                    "Insufficient stock for bundle sale: BundleId={BundleId}, Requested={Requested}, Available={Available}",
                    request.BundleId,
                    request.Quantity,
                    stockCalc.MaxAvailableBundles);

                throw new InvalidOperationException(
                    $"Insufficient stock. Requested: {request.Quantity}, Available: {stockCalc.MaxAvailableBundles}");
            }

            // Begin transaction
            _logger.LogDebug("Beginning database transaction for bundle sale");
            await _unitOfWork.BeginTransactionAsync();

            var bundle = await _unitOfWork.Bundles.GetBundleWithItemsAsync(request.BundleId);
            var stockDeductions = new List<StockDeductionInfo>();

            foreach (var item in bundle.Items)
            {
                var requiredQuantity = item.Quantity * request.Quantity;
                var stock = await GetStockForItem(item, request.WarehouseId);

                var deduction = new StockDeductionInfo
                {
                    ItemName = await GetItemName(item),
                    ItemSku = await GetItemSKU(item),
                    QuantityDeducted = requiredQuantity,
                    StockBefore = stock.Quantity,
                    StockAfter = stock.Quantity - requiredQuantity
                };

                // Log each stock deduction
                _logger.LogDebug(
                    "Deducting stock: Item={ItemName}, Before={Before}, Deduct={Deduct}, After={After}",
                    deduction.ItemName,
                    deduction.StockBefore,
                    deduction.QuantityDeducted,
                    deduction.StockAfter);

                stock.Quantity -= requiredQuantity;
                stock.UpdatedAt = DateTime.UtcNow;

                stockDeductions.Add(deduction);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var transactionId = $"TXN-{DateTime.Now:yyyyMMdd-HHmmss}";

            // Log successful transaction
            _logger.LogInformation(
                "Bundle sale completed successfully: TransactionId={TransactionId}, BundleId={BundleId}, Quantity={Quantity}, DeductedItems={ItemCount}",
                transactionId,
                request.BundleId,
                request.Quantity,
                stockDeductions.Count);

            return new SellBundleResponse
            {
                TransactionId = transactionId,
                StockDeductions = stockDeductions,
                RemainingBundleStock = stockCalc.MaxAvailableBundles - request.Quantity
            };
        }
        catch (Exception ex)
        {
            // Log transaction failure - CRITICAL
            _logger.LogError(ex,
                "CRITICAL: Bundle sale transaction failed and rolled back: BundleId={BundleId}, Quantity={Quantity}",
                request.BundleId,
                request.Quantity);

            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

---

## 3. ⚠️ Infrastructure Layer (ใช้เมื่อจำเป็น)

### ทำไมต้องระวัง:
- อาจจะ log มากเกินไป (verbose)
- ส่วนใหญ่ควรใช้ EF Core logging แทน
- ใช้เฉพาะกรณีที่มีปัญหา performance หรือ data access issues

### ตัวอย่าง: UnitOfWork (Optional)

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        ApplicationDbContext context,
        ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            // Optional: Log เฉพาะเมื่อต้องการ debug
            var changes = await _context.SaveChangesAsync();
            
            if (changes > 0)
            {
                _logger.LogDebug("Saved {ChangeCount} changes to database", changes);
            }
            
            return changes;
        }
        catch (DbUpdateException ex)
        {
            // Log database errors
            _logger.LogError(ex,
                "Database update failed: {ErrorMessage}",
                ex.InnerException?.Message ?? ex.Message);
            throw;
        }
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
        {
            _logger.LogWarning("Transaction already in progress");
            return;
        }

        _transaction = await _context.Database.BeginTransactionAsync();
        _logger.LogDebug("Database transaction started");
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            _logger.LogWarning("No transaction to commit");
            return;
        }

        try
        {
            await _transaction.CommitAsync();
            _logger.LogDebug("Database transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit database transaction");
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            _logger.LogWarning("No transaction to rollback");
            return;
        }

        try
        {
            await _transaction.RollbackAsync();
            _logger.LogWarning("Database transaction rolled back");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rollback database transaction");
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
```

### Repositories (โดยทั่วไปไม่ต้องใช้)

```csharp
// ❌ ไม่แนะนำ - Log ทุก query จะทำให้ log เยอะเกินไป
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductRepository> _logger; // ไม่แนะนำ

    public async Task<ProductMaster?> GetByIdAsync(int id)
    {
        // ❌ ไม่ต้อง log ทุก query
        _logger.LogDebug("Querying product {ProductId}", id);
        return await _context.ProductMasters.FindAsync(id);
    }
}

// ✅ แนะนำ - ใช้ EF Core logging แทน
// Configure ใน appsettings.json:
// "Microsoft.EntityFrameworkCore": "Information"
```

---

## 4. ❌ Domain Layer (ไม่ต้องใช้)

### ทำไมไม่ต้องใช้:
- Domain entities ควรเป็น pure POCO classes
- ไม่ควรมี dependencies ต่อ infrastructure
- ไม่มี business logic ที่ซับซ้อนพอที่ต้อง log

```csharp
// ❌ ไม่แนะนำ
public class ProductMaster
{
    private readonly ILogger _logger; // ไม่ควรมีใน Domain Entity

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// ✅ ถูกต้อง - Domain entities ไม่มี dependencies
public class ProductMaster
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

---

## 📋 สรุปการใช้งาน Serilog

### ✅ ใช้แน่นอน (Must Use):

1. **API Controllers**
   - ✅ Log every incoming request (via UseSerilogRequestLogging)
   - ✅ Log business operations (create, update, delete)
   - ✅ Log validation errors (Warning)
   - ✅ Log exceptions (Error)
   - ✅ Log not found scenarios (Warning)
   - ✅ Log performance issues (Warning for slow requests)

2. **Application Services**
   - ✅ Log complex business logic execution
   - ✅ Log database transactions (begin, commit, rollback)
   - ✅ Log algorithm execution (batch operations)
   - ✅ Log critical operations (transactions, stock deductions)
   - ✅ Log business rule violations
   - ✅ Log performance metrics

### ⚠️ ใช้เมื่อจำเป็น (Use When Needed):

3. **Infrastructure Layer**
   - ⚠️ UnitOfWork: Transaction lifecycle, errors
   - ⚠️ Database errors: DbUpdateException, connection issues
   - ❌ Repositories: ใช้ EF Core logging แทน

### ❌ ไม่ต้องใช้ (Don't Use):

4. **Domain Layer**
   - ❌ Entities ไม่ควรมี logging
   - ❌ Value Objects ไม่มี dependencies

---

## 🎯 Log Level Guidelines

### Debug (Development Only)
```csharp
_logger.LogDebug("Detailed info: {Detail}", detail);
```
- Algorithm steps
- Detailed data flow
- Method parameters
- Query details

### Information (Normal Operations)
```csharp
_logger.LogInformation("Operation completed: {Result}", result);
```
- Business operations completed
- API requests/responses (via middleware)
- Successful transactions
- Performance metrics

### Warning (Expected Issues)
```csharp
_logger.LogWarning("Business rule violation: {Message}", message);
```
- Business validation failures
- Not found scenarios
- Slow operations (>1000ms)
- Stock insufficient
- Retry attempts

### Error (Unexpected Issues)
```csharp
_logger.LogError(ex, "Operation failed: {Operation}", operation);
```
- Unhandled exceptions
- Database errors
- External service failures
- Transaction rollbacks

### Fatal (Critical Failures)
```csharp
Log.Fatal(ex, "Application startup failed");
```
- Application startup failures
- Configuration errors
- Critical infrastructure issues

---

## 💡 Best Practices

### 1. ใช้ Structured Logging
```csharp
// ✅ ดี - Structured properties
_logger.LogInformation(
    "Generated {VariantCount} variants in {ElapsedMs}ms",
    variantCount,
    elapsedMs);

// ❌ ไม่ดี - String interpolation
_logger.LogInformation($"Generated {variantCount} variants in {elapsedMs}ms");
```

### 2. Log ข้อมูลที่สำคัญ
```csharp
// ✅ ดี - เพียงพอสำหรับ troubleshooting
_logger.LogInformation(
    "Bundle sale: BundleId={BundleId}, Qty={Quantity}, TxnId={TransactionId}",
    bundleId,
    quantity,
    transactionId);

// ❌ มากเกินไป - Sensitive data
_logger.LogInformation(
    "User {UserId} purchased with credit card {CardNumber}",
    userId,
    cardNumber); // ❌ อย่า log sensitive data
```

### 3. Log ที่จุดสำคัญ
```csharp
public async Task<SellBundleResponse> SellBundle(SellBundleRequest request)
{
    // ✅ Log เริ่มต้น operation
    _logger.LogInformation("Starting bundle sale: BundleId={BundleId}", request.BundleId);
    
    try
    {
        // ✅ Log transaction lifecycle
        await _unitOfWork.BeginTransactionAsync();
        
        // Business logic...
        
        await _unitOfWork.CommitTransactionAsync();
        
        // ✅ Log success พร้อม metrics
        _logger.LogInformation(
            "Bundle sale completed: TransactionId={TransactionId}, ElapsedMs={ElapsedMs}",
            transactionId,
            elapsed);
        
        return response;
    }
    catch (Exception ex)
    {
        // ✅ Log error และ rollback
        _logger.LogError(ex, "Bundle sale failed: BundleId={BundleId}", request.BundleId);
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

### 4. ใช้ Exception ใน Log
```csharp
// ✅ ดี - Log exception object
_logger.LogError(ex, "Operation failed for {EntityId}", entityId);

// ❌ ไม่ดี - แค่ message
_logger.LogError("Operation failed: " + ex.Message);
```

### 5. Consistent Naming
```csharp
// ✅ ใช้ชื่อ property ที่สม่ำเสมอ
_logger.LogInformation("Created product {ProductId}", productId);
_logger.LogInformation("Updated product {ProductId}", productId);
_logger.LogInformation("Deleted product {ProductId}", productId);

// ✅ ใช้ suffix สำหรับ unit
_logger.LogInformation("Processing took {ElapsedMs}ms", elapsed);
_logger.LogInformation("File size: {FileSizeBytes} bytes", size);
```

---

## 🔍 ตัวอย่าง Log Output

### Successful Operation
```
[21:30:15 INF] Starting variant generation for product 5 with 2 options {"ProductId": 5, "OptionCount": 2}
[21:30:15 INF] Generated 12 combinations for product 5 {"CombinationCount": 12, "ProductId": 5}
[21:30:15 DBG] Beginning database transaction for variant generation
[21:30:15 INF] Successfully generated 12 variants for product 5 in 234ms {"VariantCount": 12, "ProductId": 5, "ElapsedMs": 234}
[21:30:15 INF] HTTP POST /api/products/5/generate-variants responded 200 in 245.6789ms
```

### Error Scenario
```
[21:31:20 INF] Starting bundle sale: BundleId=10, Quantity=20 {"BundleId": 10, "Quantity": 20}
[21:31:20 WRN] Insufficient stock for bundle sale: BundleId=10, Requested=20, Available=15 {"BundleId": 10, "Requested": 20, "Available": 15}
[21:31:20 INF] HTTP POST /api/bundles/10/sell responded 400 in 156.2345ms
```

### Critical Error
```
[21:32:30 INF] Processing bundle sale: BundleId=10, Quantity=5 {"BundleId": 10, "Quantity": 5}
[21:32:30 DBG] Beginning database transaction for bundle sale
[21:32:30 ERR] CRITICAL: Bundle sale transaction failed and rolled back: BundleId=10, Quantity=5 {"BundleId": 10, "Quantity": 5}
System.Data.SqlClient.SqlException: Timeout expired
   at System.Data.SqlClient.SqlCommand.ExecuteNonQuery()
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.Execute()
[21:32:30 WRN] Database transaction rolled back
[21:32:30 INF] HTTP POST /api/bundles/10/sell responded 500 in 30045.6789ms
```

---

## 📊 Summary Table

| Layer | Usage | Log Level | Examples |
|-------|-------|-----------|----------|
| **Controllers** | ✅ Always | Info, Warning, Error | Requests, responses, validation errors |
| **Services** | ✅ Always | Info, Warning, Error | Business logic, transactions, algorithms |
| **Infrastructure** | ⚠️ When needed | Debug, Error | Transaction lifecycle, DB errors |
| **Repositories** | ❌ Avoid | - | Use EF Core logging |
| **Domain** | ❌ Never | - | Pure POCO classes |

---

**สรุป**: ใช้ Serilog ใน **Controllers** และ **Services** เป็นหลัก โดยเน้น log ที่จุดสำคัญ พร้อม structured properties สำหรับ troubleshooting และ monitoring! 🎉
