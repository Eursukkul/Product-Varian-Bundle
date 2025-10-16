# 🎯 Serilog ควรใช้งานที่ไหนบ้าง - Quick Guide

## ✅ ใช้แน่นอน (Must Use)

### 1. **API Controllers** ⭐⭐⭐⭐⭐
**Priority: สูงสุด**

```csharp
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    // ✅ Log every important operation
    [HttpPost("{id}/generate-variants")]
    public async Task<IActionResult> GenerateVariants(int id, GenerateVariantsRequest request)
    {
        _logger.LogInformation(
            "Starting variant generation for product {ProductId} with {OptionCount} options",
            id, request.SelectedOptions.Count);
        
        try
        {
            var result = await _productService.GenerateVariantsAsync(request);
            
            _logger.LogInformation(
                "Generated {VariantCount} variants in {ElapsedMs}ms",
                result.TotalVariantsGenerated,
                result.ProcessingTime.TotalMilliseconds);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for product {ProductId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating variants for product {ProductId}", id);
            return StatusCode(500);
        }
    }
}
```

**Log ที่จุดเหล่านี้:**
- ✅ เริ่มต้น operation สำคัญ (POST, PUT, DELETE)
- ✅ สำเร็จพร้อม metrics (เวลา, จำนวน records)
- ✅ Validation errors → `LogWarning`
- ✅ Exceptions → `LogError`
- ✅ Not Found → `LogWarning`

---

### 2. **Application Services** ⭐⭐⭐⭐⭐
**Priority: สูงสุด**

```csharp
public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;

    public async Task<GenerateVariantsResponse> GenerateVariantsAsync(GenerateVariantsRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Starting variant generation for product {ProductId}",
            request.ProductMasterId);

        try
        {
            // Validate
            if (combinations.Count > 250)
            {
                _logger.LogWarning(
                    "Variant count {Count} exceeds limit for product {ProductId}",
                    combinations.Count, request.ProductMasterId);
                throw new InvalidOperationException("Too many variants");
            }

            // Transaction
            _logger.LogDebug("Beginning database transaction");
            await _unitOfWork.BeginTransactionAsync();
            
            // Business logic...
            
            await _unitOfWork.CommitTransactionAsync();
            stopwatch.Stop();

            _logger.LogInformation(
                "Generated {VariantCount} variants in {ElapsedMs}ms",
                variants.Count, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to generate variants for product {ProductId}",
                request.ProductMasterId);
            
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

**Log ที่จุดเหล่านี้:**
- ✅ เริ่มต้น complex operations
- ✅ Business rule validation → `LogWarning`
- ✅ Database transactions (begin/commit/rollback)
- ✅ สำเร็จพร้อม performance metrics
- ✅ Errors พร้อม context

---

## ⚠️ ใช้เมื่อจำเป็น (Use When Needed)

### 3. **Infrastructure - UnitOfWork** ⭐⭐⭐
**Priority: ปานกลาง**

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ILogger<UnitOfWork> _logger;

    public async Task BeginTransactionAsync()
    {
        _logger.LogDebug("Database transaction started");
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _transaction.CommitAsync();
            _logger.LogDebug("Transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit transaction");
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction.RollbackAsync();
        _logger.LogWarning("Transaction rolled back");
    }
}
```

**Log เฉพาะ:**
- ⚠️ Transaction lifecycle (Debug level)
- ⚠️ Database errors (Error level)
- ⚠️ Rollback events (Warning level)

---

## ❌ ไม่ต้องใช้ (Don't Use)

### 4. **Repositories** ⭐ (ใช้ EF Core Logging แทน)

```csharp
// ❌ ไม่แนะนำ - Log ทุก query จะเยอะเกินไป
public class ProductRepository : IProductRepository
{
    // ❌ ไม่ต้อง inject ILogger
    public async Task<Product> GetByIdAsync(int id)
    {
        // ❌ ไม่ต้อง log ทุก query
        return await _context.Products.FindAsync(id);
    }
}
```

**ใช้ EF Core Logging แทน** (`appsettings.json`):
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Override": {
        "Microsoft.EntityFrameworkCore": "Information"
      }
    }
  }
}
```

---

### 5. **Domain Entities** ❌ (Never)

```csharp
// ❌ ไม่มี logging ใน Domain
public class ProductMaster
{
    public int Id { get; set; }
    public string Name { get; set; }
    // Pure POCO - no dependencies
}
```

---

## 📊 สรุปตาราง

| Layer | ใช้หรือไม่ | Priority | Log Level |
|-------|----------|----------|-----------|
| **Controllers** | ✅ ใช้แน่นอน | ⭐⭐⭐⭐⭐ | Info, Warning, Error |
| **Services** | ✅ ใช้แน่นอน | ⭐⭐⭐⭐⭐ | Debug, Info, Warning, Error |
| **UnitOfWork** | ⚠️ เมื่อจำเป็น | ⭐⭐⭐ | Debug, Warning, Error |
| **Repositories** | ❌ ใช้ EF Core | ⭐ | - |
| **Domain** | ❌ ไม่ใช้ | - | - |

---

## 🎯 Log Level Guidelines

### 📘 Debug (Development Only)
```csharp
_logger.LogDebug("Computing Cartesian product for {OptionCount} options", count);
```
- Algorithm steps
- Detailed flow
- Query details

### 📗 Information (Normal Operations)
```csharp
_logger.LogInformation("Generated {VariantCount} variants in {ElapsedMs}ms", count, ms);
```
- Operation started/completed
- Success with metrics
- Business milestones

### 📙 Warning (Expected Issues)
```csharp
_logger.LogWarning("Product {ProductId} not found", id);
```
- Validation failures
- Not found
- Slow operations (>1000ms)
- Business rule violations

### 📕 Error (Unexpected Issues)
```csharp
_logger.LogError(ex, "Failed to process order {OrderId}", orderId);
```
- Exceptions
- Database errors
- Transaction failures

### 🔴 Fatal (Critical)
```csharp
Log.Fatal(ex, "Application startup failed");
```
- App startup failures
- Critical infrastructure issues

---

## 💡 Best Practices

### 1. ใช้ Structured Logging ✅
```csharp
// ✅ ดี
_logger.LogInformation("Created product {ProductId} with {VariantCount} variants", id, count);

// ❌ ไม่ดี
_logger.LogInformation($"Created product {id} with {count} variants");
```

### 2. Log ที่จุดสำคัญ ✅
```csharp
// ✅ เริ่มต้น operation
_logger.LogInformation("Starting bundle sale for {BundleId}", bundleId);

// ทำงาน...

// ✅ สำเร็จ พร้อม metrics
_logger.LogInformation("Sale completed in {ElapsedMs}ms", elapsed);
```

### 3. Include Exception ✅
```csharp
// ✅ ดี - มี exception details
_logger.LogError(ex, "Failed to process {EntityId}", id);

// ❌ ไม่ดี - แค่ message
_logger.LogError("Failed: " + ex.Message);
```

### 4. Consistent Property Names ✅
```csharp
// ✅ ใช้ชื่อเดียวกันทุกที่
_logger.LogInformation("Created product {ProductId}", id);
_logger.LogInformation("Updated product {ProductId}", id);
_logger.LogInformation("Deleted product {ProductId}", id);
```

### 5. ระวัง Sensitive Data ❌
```csharp
// ❌ อย่า log sensitive data
_logger.LogInformation("User {Email} paid with card {CardNumber}", email, card);

// ✅ Log แค่ ID
_logger.LogInformation("Payment processed for user {UserId}", userId);
```

---

## 📋 Checklist สำหรับ FlowAccount API

### Controllers ที่ควร Log:
- [x] ProductsController
  - [x] GetAll, GetById, Create, Update, Delete
  - [x] **GenerateVariants** (Batch Operation) ⭐
- [x] BundlesController
  - [x] GetAll, GetById, Create, Update, Delete
  - [x] **CalculateStock** (Stock Logic) ⭐
  - [x] **SellBundle** (Transaction) ⭐

### Services ที่ควร Log:
- [ ] ProductService
  - [ ] CreateProductAsync
  - [ ] UpdateProductAsync
  - [ ] DeleteProductAsync
  - [ ] **GenerateVariantsAsync** (Complex Algorithm) ⭐⭐⭐
- [ ] BundleService
  - [ ] CreateBundleAsync
  - [ ] UpdateBundleAsync
  - [ ] DeleteBundleAsync
  - [ ] **CalculateBundleStockAsync** (Stock Logic) ⭐⭐⭐
  - [ ] **SellBundleAsync** (Transaction) ⭐⭐⭐

### Infrastructure:
- [ ] UnitOfWork
  - [ ] BeginTransactionAsync
  - [ ] CommitTransactionAsync
  - [ ] RollbackTransactionAsync

---

## 🚀 ตัวอย่าง Output ที่คาดหวัง

### Successful Request:
```
[21:30:15 INF] HTTP POST /api/products/5/generate-variants started
[21:30:15 INF] Starting variant generation for product 5 with 2 options
[21:30:15 DBG] Computing Cartesian product for 2 options
[21:30:15 DBG] Beginning database transaction
[21:30:15 INF] Generated 12 variants in 234ms
[21:30:15 INF] HTTP POST /api/products/5/generate-variants responded 200 in 245.67ms
```

### Error Scenario:
```
[21:31:20 INF] HTTP POST /api/bundles/10/sell started
[21:31:20 INF] Processing bundle sale: BundleId=10, Quantity=20
[21:31:20 WRN] Insufficient stock: BundleId=10, Requested=20, Available=15
[21:31:20 INF] HTTP POST /api/bundles/10/sell responded 400 in 156.23ms
```

### Critical Error:
```
[21:32:30 INF] Processing bundle sale: BundleId=10, Quantity=5
[21:32:30 DBG] Beginning database transaction
[21:32:30 ERR] Failed to complete transaction: BundleId=10
System.Data.SqlClient.SqlException: Timeout expired
   at System.Data.SqlClient.SqlCommand.ExecuteNonQuery()
[21:32:30 WRN] Transaction rolled back
[21:32:30 INF] HTTP POST /api/bundles/10/sell responded 500 in 30045.67ms
```

---

## 📚 เอกสารเพิ่มเติม

- 📄 `SERILOG_CONFIGURATION.md` - Setup และ configuration
- 📄 `SERILOG_BEST_PRACTICES.md` - ตัวอย่างโค้ดละเอียด
- 📄 `TASK2_API_DESIGN.md` - API documentation

---

**สรุป**: ใช้ Serilog ใน **Controllers** และ **Services** เป็นหลัก โดยเน้น:
- ✅ Structured logging (`{PropertyName}`)
- ✅ Log ที่จุดสำคัญ (start, success, error)
- ✅ Include metrics (elapsed time, counts)
- ✅ Proper log levels (Info, Warning, Error)
- ❌ อย่า log sensitive data
- ❌ อย่า log ทุก query (ใช้ EF Core logging)

🎉 **Serilog พร้อมใช้งานใน FlowAccount API แล้ว!**
