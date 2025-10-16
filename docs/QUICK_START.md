# FlowAccount API - Quick Start Guide

## ✅ Setup Complete!

Your FlowAccount Inventory Management API is ready to use with full Swagger documentation.

---

## 🚀 Starting the API

### 1. Navigate to API Project

```powershell
cd c:\Users\Chalermphan\source\flowaccout\src\FlowAccount.API
```

### 2. Run the API

```powershell
dotnet run
```

### 3. Access Swagger UI

Open your browser and go to:

**http://localhost:5159**

---

## 📖 What You Can Do

### ✅ Interactive API Testing

1. **Browse Endpoints** - See all 14 API endpoints organized by category
2. **Try It Out** - Test any endpoint directly from the browser
3. **View Examples** - See request/response structures
4. **Copy Requests** - Get curl commands for your own code

### ✅ Available Features

- 🏷️ **Products** - Create products with variant options (Color, Size, etc.)
- 🎨 **Variants** - Generate all combinations automatically (Cartesian product)
- 📦 **Bundles** - Create product bundles with multiple items
- 📊 **Stock** - Manage inventory across warehouses
- 🔄 **Transactions** - Atomic stock deductions with rollback support

---

## 📋 Quick API Examples

### Create a Product with Variants

**Endpoint:** `POST /api/products`

```json
{
  "name": "T-Shirt",
  "description": "Cotton T-Shirt",
  "categoryId": 1,
  "isActive": true,
  "variantOptions": [
    {
      "name": "Color",
      "displayOrder": 1,
      "values": ["Red", "Blue", "Green"]
    },
    {
      "name": "Size",
      "displayOrder": 2,
      "values": ["S", "M", "L", "XL"]
    }
  ]
}
```

This creates a product with 2 variant options.

### Generate All Variants (Cartesian Product)

**Endpoint:** `POST /api/products/1/generate-variants`

```json
{
  "productMasterId": 1,
  "selectedOptions": {
    "1": [1, 2, 3],
    "2": [1, 2, 3, 4]
  },
  "basePrice": 299.00,
  "baseCost": 150.00,
  "priceStrategy": "FixedPrice",
  "skuPattern": "TSHIRT-{Color}-{Size}"
}
```

This generates 12 variants (3 colors × 4 sizes) automatically!

Result: TSHIRT-RED-S, TSHIRT-RED-M, TSHIRT-RED-L, etc.

### Create a Bundle

**Endpoint:** `POST /api/bundles`

```json
{
  "name": "Premium Bundle",
  "description": "2 T-Shirts + 1 Cap",
  "price": 599.00,
  "isActive": true,
  "items": [
    {
      "itemType": "Variant",
      "itemId": 1,
      "quantity": 2
    },
    {
      "itemType": "Variant",
      "itemId": 5,
      "quantity": 1
    }
  ]
}
```

### Check Bundle Stock

**Endpoint:** `POST /api/bundles/calculate-stock`

```json
{
  "bundleId": 1,
  "warehouseId": 1
}
```

**Response includes:**
- Maximum bundles that can be sold
- Stock breakdown per item
- Bottleneck detection (which item is limiting)
- Detailed explanation

### Sell a Bundle

**Endpoint:** `POST /api/bundles/sell`

```json
{
  "bundleId": 1,
  "warehouseId": 1,
  "quantity": 2,
  "allowBackorder": false
}
```

This will:
- ✅ Validate stock availability
- ✅ Start database transaction
- ✅ Deduct stock for each item atomically
- ✅ Commit or rollback on error
- ✅ Return transaction details with stock changes

---

## 🏗️ Architecture Overview

### Clean Architecture Structure

```
FlowAccount
├── Domain (Entities, Interfaces)
│   └── No dependencies
├── Application (Services, DTOs)
│   └── Depends on: Domain
├── Infrastructure (DbContext, Repositories)
│   └── Depends on: Domain
└── API (Controllers, Swagger)
    └── Depends on: Application, Infrastructure
```

### Key Design Patterns

✅ **Repository Pattern** - Data access abstraction  
✅ **Unit of Work** - Transaction management  
✅ **Dependency Injection** - Loose coupling  
✅ **DTO Pattern** - Separate API models from domain  
✅ **AutoMapper** - Object-to-object mapping  
✅ **Structured Logging** - Serilog with file rotation  

---

## 📊 Database Schema

### Core Tables

- **ProductMasters** - Product definitions with variant options
- **VariantOptions** - Option types (Color, Size)
- **VariantOptionValues** - Option values (Red, Blue, S, M, L)
- **ProductVariants** - Actual sellable products (T-Shirt Red M)
- **Bundles** - Product bundles
- **BundleItems** - Items in each bundle
- **Stock** - Inventory levels per warehouse
- **StockTransactions** - All stock movements (audit trail)

### Business Logic

**Variant Generation:**
- Cartesian product algorithm
- Generates up to 250 variants
- Automatic SKU generation
- Price calculation strategies (Fixed, Add, Multiply)

**Bundle Stock Calculation:**
- Queries stock for each item
- Calculates maximum bundles
- Identifies bottlenecks
- Provides detailed breakdown

**Bundle Sales:**
- Transaction-based stock deduction
- Atomic operations (all or nothing)
- Automatic rollback on error
- Complete audit trail

---

## 📝 Logging

### Serilog Configuration

✅ **Console Logs** - Development debugging  
✅ **File Logs** - Production audit trail  
✅ **Structured Logging** - JSON properties for querying  
✅ **Daily Rotation** - New file every day  
✅ **30-Day Retention** - Automatic cleanup  

### Log Locations

**Console:** Terminal output  
**Files:** `logs/flowaccount-YYYYMMDD.log`

### What's Logged

- ✅ All CRUD operations (start, success, failure)
- ✅ Complex algorithms (variant generation, stock calculations)
- ✅ Transactions (start, commit, rollback)
- ✅ HTTP requests (method, path, duration, status code)
- ✅ Business events (bottleneck detection, stock deductions)
- ✅ Performance metrics (algorithm duration in ms)

---

## 🔒 Security (Ready for Enhancement)

### Current Setup

✅ **CORS Enabled** - AllowAll policy (development)  
✅ **HTTPS Redirect** - Enforced in production  
✅ **Request Logging** - All requests tracked  

### Future Enhancements

- [ ] JWT Authentication
- [ ] Role-based Authorization
- [ ] API Rate Limiting
- [ ] Input Validation (FluentValidation)
- [ ] CORS restriction for production

---

## 📖 Documentation Files

Created documentation:

1. ✅ **SWAGGER_DOCUMENTATION.md** - Complete Swagger guide
2. ✅ **SERILOG_CONFIGURATION.md** - Logging setup
3. ✅ **SERILOG_BEST_PRACTICES.md** - 800+ lines of examples
4. ✅ **SERILOG_USAGE_GUIDE.md** - Quick reference
5. ✅ **SERILOG_IMPLEMENTATION_SUMMARY.md** - Implementation details
6. ✅ **QUICK_START.md** - This file

---

## 🐛 Troubleshooting

### API doesn't start

**Check:**
1. SQL Server is running
2. Connection string in `appsettings.json` is correct
3. Database migration applied: `dotnet ef database update`

### Swagger UI blank

**Check:**
1. Running in Development mode: `ASPNETCORE_ENVIRONMENT=Development`
2. Navigate to root: http://localhost:5159 (not /swagger)

### Database errors

**Solution:**
```powershell
cd c:\Users\Chalermphan\source\flowaccout\src\FlowAccount.Infrastructure
dotnet ef database update --startup-project ../FlowAccount.API
```

---

## 🎯 Next Steps

### Recommended Enhancements

1. **Add Authentication**
   - Install JWT packages
   - Configure authentication middleware
   - Add [Authorize] attributes

2. **Add Validation**
   - FluentValidation already installed
   - Create validators for requests
   - Add validation middleware

3. **Add Caching**
   - Redis for distributed cache
   - Memory cache for frequently accessed data

4. **Add Testing**
   - xUnit for unit tests
   - Moq for mocking
   - Integration tests for API endpoints

5. **Performance Optimization**
   - Add indexes to database
   - Implement pagination
   - Add response compression

---

## 📊 Project Status

### ✅ Completed Features

- [x] Clean Architecture setup
- [x] Database schema with 8 tables
- [x] Repository pattern with Unit of Work
- [x] Service layer with business logic
- [x] 14 REST API endpoints
- [x] Serilog structured logging
- [x] Swagger documentation
- [x] CORS configuration
- [x] Complex algorithms (Cartesian product, stock calculation)
- [x] Transaction management with rollback

### 🎯 Production Ready

✅ Database migrations  
✅ Error handling  
✅ Logging infrastructure  
✅ API documentation  
✅ Transaction safety  
✅ Stock management  
✅ Bundle calculations  

---

## 🌐 URLs

**API Base:** http://localhost:5159  
**Swagger UI:** http://localhost:5159  
**Health Check:** http://localhost:5159/health  
**Swagger JSON:** http://localhost:5159/swagger/v1/swagger.json  

---

## 💡 Tips

### Testing with Swagger

1. **Start from top to bottom:**
   - Create a category first (if needed)
   - Create a product
   - Generate variants
   - Adjust stock
   - Create bundles
   - Calculate stock
   - Sell bundles

2. **Copy Response IDs:**
   - After creating a product, note the `id`
   - Use that ID when generating variants
   - Use variant IDs when creating bundles

3. **Watch the Logs:**
   - Console shows real-time activity
   - See structured properties
   - Monitor performance

### Working with the API

- All POST/PUT endpoints return created/updated entities
- DELETE endpoints return boolean (true/false)
- Stock calculations show bottleneck detection
- Bundle sales are transactional (atomic)
- All operations are logged with structured data

---

**Status:** 🎉 Ready to Use!  
**Swagger:** ✅ Configured  
**Logging:** ✅ Working  
**Database:** ✅ Migrated  
**Endpoints:** ✅ 14 Available  
**Documentation:** ✅ Complete
