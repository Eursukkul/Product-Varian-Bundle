# Task 2: API Endpoint Design and Business Logic

## ✅ สถานะความสำเร็จ
- **Domain Layer**: ✅ สร้างแล้ว (10 Entities, 6 Interfaces)
- **Infrastructure Layer**: ✅ สร้างแล้ว (DbContext, 11 Configurations, 6 Repositories, UnitOfWork)
- **Database Migration**: ✅ ใช้งานได้ (FlowAccountDb with 10 tables)
- **Application Layer**: ✅ สร้างแล้ว (8 DTOs, 2 Services, AutoMapper)
- **API Layer**: ✅ สร้างแล้ว (2 Controllers with 14 endpoints)
- **Server Status**: ✅ Running on `http://localhost:5159`

---

## 📋 API Endpoints Overview

### Products API (`/api/products`)
| Method | Endpoint | Description | Special Feature |
|--------|----------|-------------|-----------------|
| GET | `/api/products` | Get all products with variants | - |
| GET | `/api/products/{id}` | Get product by ID | - |
| POST | `/api/products` | Create product with variant options | - |
| PUT | `/api/products/{id}` | Update product | - |
| DELETE | `/api/products/{id}` | Delete product | - |
| **POST** | **`/api/products/{id}/generate-variants`** | **Batch generate variants** | **🚀 BATCH OPERATION** |

### Bundles API (`/api/bundles`)
| Method | Endpoint | Description | Special Feature |
|--------|----------|-------------|-----------------|
| GET | `/api/bundles` | Get all bundles | - |
| GET | `/api/bundles/{id}` | Get bundle by ID | - |
| POST | `/api/bundles` | Create bundle with items | - |
| PUT | `/api/bundles/{id}` | Update bundle | - |
| DELETE | `/api/bundles/{id}` | Delete bundle | - |
| **POST** | **`/api/bundles/{id}/calculate-stock`** | **Calculate bundle availability** | **📊 STOCK LOGIC** |
| **POST** | **`/api/bundles/{id}/sell`** | **Sell bundle (atomic stock deduction)** | **💰 TRANSACTION MANAGEMENT** |

---

## 🎯 Core Features Implementation

### 1. 🚀 BATCH OPERATION: Generate Variants (Up to 250 variants)

**Endpoint**: `POST /api/products/{id}/generate-variants`

**Algorithm**: Cartesian Product
- Input: Selected variant options (Size, Color, etc.)
- Process: Generate all combinations using recursive algorithm
- Limit: Maximum 250 variants per batch
- Output: Total variants created + Processing time

**Request Example**:
```json
POST http://localhost:5159/api/products/5/generate-variants
Content-Type: application/json

{
  "productMasterId": 5,
  "selectedOptions": {
    "1": [1, 2, 3, 4],     // Size option: S, M, L, XL (VariantOptionValue IDs)
    "2": [5, 6, 7]         // Color option: Red, Blue, Green
  },
  "priceStrategy": "SizeAdjusted",
  "basePrice": 299.00,
  "baseCost": 150.00,
  "skuPattern": "{ProductSKU}-{Size}-{Color}"
}
```

**Calculation**: 4 sizes × 3 colors = **12 variants**

**Response Example**:
```json
{
  "success": true,
  "message": "Successfully generated 12 variants in 125.45ms",
  "data": {
    "productMasterId": 5,
    "productName": "T-Shirt Premium",
    "totalVariantsGenerated": 12,
    "processingTime": "00:00:00.1254500",
    "variants": [
      {
        "id": 101,
        "sku": "TS-001-S-Red",
        "price": 299.00,
        "cost": 150.00,
        "attributes": [
          { "optionName": "Size", "optionValue": "S" },
          { "optionName": "Color", "optionValue": "Red" }
        ]
      },
      {
        "id": 102,
        "sku": "TS-001-S-Blue",
        "price": 299.00,
        "cost": 150.00,
        "attributes": [
          { "optionName": "Size", "optionValue": "S" },
          { "optionName": "Color", "optionValue": "Blue" }
        ]
      }
      // ... (10 more variants)
    ],
    "message": "Successfully generated 12 product variants"
  }
}
```

**Price Strategies**:
- `Fixed`: All variants same price = basePrice
- `SizeAdjusted`: S=base, M=base+20, L=base+40, XL=base+60
- `ColorAdjusted`: Custom adjustments per color

**SKU Pattern Support**:
- `{ProductSKU}`: Master product SKU
- `{Size}`, `{Color}`, etc.: Option value names
- Example: `{ProductSKU}-{Size}-{Color}` → `TS-001-M-Blue`

---

### 2. 📊 STOCK LOGIC: Calculate Bundle Availability

**Endpoint**: `POST /api/bundles/{id}/calculate-stock`

**Algorithm**: Bottleneck Detection
- For each bundle item: `possible_bundles = available_stock / required_quantity`
- Max available bundles = `MIN(all possible_bundles)`
- Identify bottleneck items (items limiting production)

**Request Example**:
```json
POST http://localhost:5159/api/bundles/10/calculate-stock
Content-Type: application/json

{
  "bundleId": 10,
  "warehouseId": 1
}
```

**Response Example**:
```json
{
  "success": true,
  "message": "Bundle can produce 15 units",
  "data": {
    "maxAvailableBundles": 15,
    "itemsStockBreakdown": [
      {
        "itemName": "T-Shirt (M, Blue)",
        "itemSku": "TS-001-M-Blue",
        "requiredQuantity": 1,
        "availableQuantity": 50,
        "possibleBundles": 50,
        "isBottleneck": false
      },
      {
        "itemName": "Shorts (L, Black)",
        "itemSku": "SHORT-001-L-Black",
        "requiredQuantity": 1,
        "availableQuantity": 15,
        "possibleBundles": 15,
        "isBottleneck": true    // ⚠️ This item limits production
      },
      {
        "itemName": "Hat",
        "itemSku": "HAT-001",
        "requiredQuantity": 2,
        "availableQuantity": 100,
        "possibleBundles": 50,  // 100 / 2 = 50
        "isBottleneck": false
      }
    ],
    "warehouseName": "Warehouse 1",
    "explanation": "Bundle availability limited by Shorts (L, Black) - only 15 units available"
  }
}
```

**Business Logic**:
1. Retrieve bundle with all items
2. For each item, query current stock in specified warehouse
3. Calculate `possibleBundles` for each item
4. Determine minimum across all items = max available bundles
5. Mark items with `possibleBundles == maxAvailableBundles` as bottlenecks
6. Generate human-readable explanation

---

### 3. 💰 TRANSACTION MANAGEMENT: Sell Bundle

**Endpoint**: `POST /api/bundles/{id}/sell`

**Algorithm**: Atomic Transaction
- Begin database transaction
- Validate stock availability (if `allowBackorder = false`)
- Deduct stock for each component item
- Record stock changes (before/after quantities)
- Commit transaction (or rollback on error)

**Transaction Guarantees**:
- **ATOMICITY**: All stock deductions succeed or all are rolled back
- **CONSISTENCY**: Stock never goes negative (unless backorder allowed)
- **ISOLATION**: Uses database transaction for concurrent safety
- **DURABILITY**: Changes persisted only after successful commit

**Request Example**:
```json
POST http://localhost:5159/api/bundles/10/sell
Content-Type: application/json

{
  "bundleId": 10,
  "warehouseId": 1,
  "quantity": 5,
  "allowBackorder": false
}
```

**Response Example**:
```json
{
  "success": true,
  "message": "Bundle sold successfully. 10 bundles remaining",
  "data": {
    "transactionId": "TXN-20240116-135827",
    "stockDeductions": [
      {
        "itemName": "T-Shirt (M, Blue)",
        "itemSku": "TS-001-M-Blue",
        "quantityDeducted": 5,
        "stockBefore": 50,
        "stockAfter": 45
      },
      {
        "itemName": "Shorts (L, Black)",
        "itemSku": "SHORT-001-L-Black",
        "quantityDeducted": 5,
        "stockBefore": 15,
        "stockAfter": 10
      },
      {
        "itemName": "Hat",
        "itemSku": "HAT-001",
        "quantityDeducted": 10,    // Required 2 per bundle × 5 bundles = 10
        "stockBefore": 100,
        "stockAfter": 90
      }
    ],
    "remainingBundleStock": 10
  }
}
```

**Error Handling**:
```json
// Insufficient stock example (allowBackorder = false)
{
  "success": false,
  "message": "Insufficient stock for bundle item: Shorts (L, Black)",
  "errors": [
    "Required: 20 units, Available: 15 units"
  ]
}
```

**Business Logic**:
1. Calculate bundle stock availability (reuse stock calculation logic)
2. Validate requested quantity ≤ available bundles (if backorder not allowed)
3. Begin database transaction via `IUnitOfWork.BeginTransactionAsync()`
4. For each bundle item:
   - Calculate required quantity = item.Quantity × bundle quantity
   - Query current stock record
   - Record `stockBefore`
   - Deduct: `stock.Quantity -= requiredQuantity`
   - Record `stockAfter`
   - Update stock in database
5. Generate unique transaction ID
6. Commit transaction via `IUnitOfWork.CommitTransactionAsync()`
7. Return detailed deduction report

---

## 🔧 Additional API Examples

### Create Product with Variant Options

**Request**:
```json
POST http://localhost:5159/api/products
Content-Type: application/json

{
  "name": "T-Shirt Premium",
  "sku": "TS-001",
  "categoryId": 1,
  "isActive": true,
  "variantOptions": [
    {
      "name": "Size",
      "displayOrder": 1,
      "values": ["S", "M", "L", "XL"]
    },
    {
      "name": "Color",
      "displayOrder": 2,
      "values": ["Red", "Blue", "Green", "Black", "White"]
    }
  ]
}
```

**Response**:
```json
{
  "success": true,
  "message": "Product created successfully",
  "data": {
    "id": 5,
    "name": "T-Shirt Premium",
    "sku": "TS-001",
    "categoryId": 1,
    "isActive": true,
    "variantOptions": [
      {
        "id": 1,
        "name": "Size",
        "displayOrder": 1,
        "values": [
          { "id": 1, "value": "S", "displayOrder": 1 },
          { "id": 2, "value": "M", "displayOrder": 2 },
          { "id": 3, "value": "L", "displayOrder": 3 },
          { "id": 4, "value": "XL", "displayOrder": 4 }
        ]
      },
      {
        "id": 2,
        "name": "Color",
        "displayOrder": 2,
        "values": [
          { "id": 5, "value": "Red", "displayOrder": 1 },
          { "id": 6, "value": "Blue", "displayOrder": 2 },
          { "id": 7, "value": "Green", "displayOrder": 3 },
          { "id": 8, "value": "Black", "displayOrder": 4 },
          { "id": 9, "value": "White", "displayOrder": 5 }
        ]
      }
    ],
    "productVariants": []
  }
}
```

---

### Create Bundle with Mixed Items

**Request**:
```json
POST http://localhost:5159/api/bundles
Content-Type: application/json

{
  "name": "Summer Collection Bundle",
  "description": "Complete summer outfit",
  "price": 799.00,
  "isActive": true,
  "items": [
    {
      "itemType": "Variant",
      "itemId": 123,    // ProductVariant ID (T-Shirt-M-Blue)
      "quantity": 1
    },
    {
      "itemType": "Variant",
      "itemId": 145,    // ProductVariant ID (Shorts-L-Black)
      "quantity": 1
    },
    {
      "itemType": "Product",
      "itemId": 8,      // ProductMaster ID (Hat - any variant)
      "quantity": 2
    }
  ]
}
```

**Response**:
```json
{
  "success": true,
  "message": "Bundle created successfully with 3 items",
  "data": {
    "bundle": {
      "id": 10,
      "name": "Summer Collection Bundle",
      "description": "Complete summer outfit",
      "price": 799.00,
      "isActive": true,
      "items": [
        {
          "itemType": "Variant",
          "itemId": 123,
          "itemName": "T-Shirt (M, Blue)",
          "itemSku": "TS-001-M-Blue",
          "quantity": 1
        },
        {
          "itemType": "Variant",
          "itemId": 145,
          "itemName": "Shorts (L, Black)",
          "itemSku": "SHORT-001-L-Black",
          "quantity": 1
        },
        {
          "itemType": "Product",
          "itemId": 8,
          "itemName": "Hat",
          "itemSku": "HAT-001",
          "quantity": 2
        }
      ]
    },
    "message": "Bundle created successfully with 3 items"
  }
}
```

---

## 🏗️ Architecture Summary

### Clean Architecture Layers

```
┌─────────────────────────────────────────┐
│         FlowAccount.API                 │  ← Controllers, Program.cs
│  - ProductsController (6 endpoints)     │
│  - BundlesController (8 endpoints)      │
└────────────────┬────────────────────────┘
                 │ depends on
┌────────────────▼────────────────────────┐
│      FlowAccount.Application            │  ← Business Logic
│  - DTOs (8 files)                       │
│  - Services (ProductService, BundleService)
│  - Interfaces (IProductService, IBundleService)
│  - AutoMapper Profile                   │
└────────────────┬────────────────────────┘
                 │ depends on
┌────────────────▼────────────────────────┐
│       FlowAccount.Domain                │  ← Core Entities
│  - Entities (10 classes)                │
│  - Interfaces (6 repository interfaces) │
└────────────────▲────────────────────────┘
                 │ implements
┌────────────────┴────────────────────────┐
│     FlowAccount.Infrastructure          │  ← Data Access
│  - ApplicationDbContext                 │
│  - Entity Configurations (11 files)     │
│  - Repositories (6 implementations)     │
│  - UnitOfWork                           │
└─────────────────────────────────────────┘
```

### Design Patterns Used

1. **Repository Pattern**: Abstraction over data access
2. **Unit of Work Pattern**: Transaction management
3. **Strategy Pattern**: Price calculation strategies
4. **Factory Pattern**: Variant generation
5. **DTO Pattern**: Data transfer between layers
6. **CQRS Pattern**: Separate read/write operations (MediatR ready)

---

## 🧪 Testing the API

### Using Swagger UI (OpenAPI)
1. Navigate to: `http://localhost:5159/openapi/v1.json`
2. Swagger documentation auto-generated from XML comments

### Using curl

```bash
# Get all products
curl http://localhost:5159/api/products

# Create product
curl -X POST http://localhost:5159/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Product",
    "sku": "TEST-001",
    "categoryId": 1,
    "isActive": true,
    "variantOptions": []
  }'

# Generate variants (Batch Operation)
curl -X POST http://localhost:5159/api/products/5/generate-variants \
  -H "Content-Type: application/json" \
  -d '{
    "productMasterId": 5,
    "selectedOptions": {"1": [1,2], "2": [3,4]},
    "priceStrategy": "Fixed",
    "basePrice": 299.00,
    "baseCost": 150.00
  }'

# Calculate bundle stock (Stock Logic)
curl -X POST http://localhost:5159/api/bundles/10/calculate-stock \
  -H "Content-Type: application/json" \
  -d '{
    "bundleId": 10,
    "warehouseId": 1
  }'

# Sell bundle (Transaction Management)
curl -X POST http://localhost:5159/api/bundles/10/sell \
  -H "Content-Type: application/json" \
  -d '{
    "bundleId": 10,
    "warehouseId": 1,
    "quantity": 5,
    "allowBackorder": false
  }'
```

---

## 📊 Database Schema

### 10 Tables Created:
1. **ProductMaster** - Main product info
2. **Categories** - Product categories
3. **VariantOptions** - Option types (Size, Color)
4. **VariantOptionValues** - Option values (S, M, L)
5. **ProductVariants** - Generated variants
6. **ProductVariantAttributes** - Variant attribute mappings
7. **Bundles** - Bundle definitions
8. **BundleItems** - Items in each bundle
9. **Warehouses** - Warehouse locations
10. **Stock** - Stock levels per warehouse

### Key Indexes (20+ total):
- Unique SKU constraints
- Foreign key indexes
- Composite indexes for variant lookup
- Performance indexes for stock queries

---

## ✅ Task 2 Completion Checklist

- [x] **API Endpoint Design**: 14 endpoints across 2 controllers
- [x] **Request/Response Payloads**: Comprehensive DTOs with examples
- [x] **Batch Operations**: Generate up to 250 variants simultaneously
  - [x] Cartesian product algorithm
  - [x] Price strategy support
  - [x] SKU pattern generation
  - [x] Transaction management
  - [x] Processing time tracking
- [x] **Transaction Management**: Atomic bundle sales
  - [x] Stock validation
  - [x] Database transactions (Begin/Commit/Rollback)
  - [x] Detailed deduction tracking
  - [x] Error handling with rollback
- [x] **Stock Logic**: Bundle availability calculation
  - [x] Bottleneck identification
  - [x] Per-item stock breakdown
  - [x] Human-readable explanation
- [x] **Clean Architecture**: Proper layer separation
- [x] **Error Handling**: Consistent error responses
- [x] **Logging**: Structured logging with LoggerFactory
- [x] **Dependency Injection**: All services registered
- [x] **AutoMapper**: Entity-DTO mapping configured
- [x] **XML Documentation**: API documentation in Swagger

---

## 🚀 Next Steps (Task 3 - Frontend)

1. Build React/Vue frontend
2. Connect to API endpoints
3. Implement UI for:
   - Product management with variant options
   - Batch variant generation interface
   - Bundle creation with item selection
   - Stock calculation dashboard
   - Bundle sales transaction form
4. Real-time stock updates
5. Error handling and validation feedback

---

## 📝 Notes

- API is running on: `http://localhost:5159`
- Database: SQL Server LocalDB `FlowAccountDb`
- .NET Version: 10.0.100-rc.2 (Preview)
- Entity Framework Core: 9.0.10
- Clean Architecture strictly enforced (Application layer never references Infrastructure)
- All complex business logic in Services (not in Controllers)
- Transaction management via IUnitOfWork pattern
- Repository pattern for data access abstraction

---

**สรุป**: Task 2 เสร็จสมบูรณ์ ✅  
- 14 API Endpoints พร้อมใช้งาน
- ครบทั้ง 3 requirements หลัก: Batch Operations, Transaction Management, Stock Logic
- พร้อมสำหรับ integration กับ Frontend (Task 3)
