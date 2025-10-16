# Swagger API Documentation

## ✅ Setup Complete

Swagger (OpenAPI) documentation has been successfully configured for the FlowAccount API.

---

## 🌐 Accessing Swagger UI

### Development Environment

**URL:** http://localhost:5159

The Swagger UI is configured to load at the **root** of the application (/) when running in Development mode.

### Features Available

✅ **Interactive API Documentation** - Browse all endpoints  
✅ **Try It Out** - Test API endpoints directly from the browser  
✅ **Request/Response Examples** - See data structures  
✅ **Schema Documentation** - View DTO models  
✅ **Request Duration** - Performance tracking enabled  

---

## 📁 API Endpoints Available

### 🏷️ Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

### 🎨 Variants

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/variants` | Get all variants |
| GET | `/api/variants/{id}` | Get variant by ID |
| POST | `/api/products/{productId}/generate-variants` | Generate variants (Cartesian product) |

### 📦 Bundles

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/bundles` | Get all bundles |
| GET | `/api/bundles/{id}` | Get bundle by ID |
| POST | `/api/bundles` | Create new bundle |
| PUT | `/api/bundles/{id}` | Update bundle |
| DELETE | `/api/bundles/{id}` | Delete bundle |
| POST | `/api/bundles/calculate-stock` | Calculate bundle stock |
| POST | `/api/bundles/sell` | Sell bundle (with stock deduction) |

### 📊 Stock

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/stock/adjust` | Adjust stock quantity |
| GET | `/api/stock/query` | Query stock levels |

### 🏥 Health

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | API health check |

---

## 🎯 Configuration Details

### Program.cs Configuration

```csharp
// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "FlowAccount API",
        Version = "v1",
        Description = "FlowAccount Inventory Management System API - Product, Variant, Bundle, and Stock Management",
        Contact = new()
        {
            Name = "FlowAccount Team",
            Email = "support@flowaccount.com"
        }
    });

    options.CustomSchemaIds(type => type.FullName);
});

// Middleware Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FlowAccount API v1");
        options.RoutePrefix = string.Empty; // Swagger at root (/)
        options.DocumentTitle = "FlowAccount API Documentation";
        options.DisplayRequestDuration();
    });
}
```

### Key Features

1. **Root Access** - `RoutePrefix = string.Empty` sets Swagger UI at `/`
2. **Request Duration** - `DisplayRequestDuration()` shows API performance
3. **Full Schema Names** - `CustomSchemaIds(type => type.FullName)` prevents naming conflicts
4. **Development Only** - Swagger only enabled in Development environment

---

## 📖 Using Swagger UI

### 1. Browse Endpoints

- Open http://localhost:5159
- See all available endpoints grouped by controller
- Click on any endpoint to expand details

### 2. Test Endpoints

1. Click on an endpoint (e.g., `GET /api/products`)
2. Click **"Try it out"** button
3. Fill in required parameters
4. Click **"Execute"**
5. See the response below

### 3. View Schemas

- Scroll down to **"Schemas"** section
- View all DTO models and their properties
- See data types, required fields, and examples

### 4. Copy Request Examples

- After testing, copy the **curl** command
- Use in your own code or API testing tools

---

## 🧪 Example API Calls via Swagger

### Create a Product

**Endpoint:** `POST /api/products`

**Request Body:**
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

### Generate Variants

**Endpoint:** `POST /api/products/{productId}/generate-variants`

**Request Body:**
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

### Create a Bundle

**Endpoint:** `POST /api/bundles`

**Request Body:**
```json
{
  "name": "Premium T-Shirt Bundle",
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

### Calculate Bundle Stock

**Endpoint:** `POST /api/bundles/calculate-stock`

**Request Body:**
```json
{
  "bundleId": 1,
  "warehouseId": 1
}
```

**Response:**
```json
{
  "bundleId": 1,
  "bundleName": "Premium T-Shirt Bundle",
  "maxAvailableBundles": 10,
  "itemsStockBreakdown": [
    {
      "itemType": "Variant",
      "itemId": 1,
      "itemName": "T-Shirt Red M",
      "itemSKU": "TSHIRT-RED-M",
      "requiredQuantity": 2,
      "availableQuantity": 50,
      "possibleBundles": 25,
      "isBottleneck": false
    },
    {
      "itemType": "Variant",
      "itemId": 5,
      "itemName": "Cap Blue",
      "itemSKU": "CAP-BLUE",
      "requiredQuantity": 1,
      "availableQuantity": 10,
      "possibleBundles": 10,
      "isBottleneck": true
    }
  ],
  "explanation": "Bundle 'Premium T-Shirt Bundle' can be sold 10 times. Limited by: Cap Blue (10 available, 1 required)"
}
```

### Sell Bundle

**Endpoint:** `POST /api/bundles/sell`

**Request Body:**
```json
{
  "bundleId": 1,
  "warehouseId": 1,
  "quantity": 2,
  "allowBackorder": false
}
```

**Response:**
```json
{
  "success": true,
  "message": "Bundle sold successfully",
  "bundleId": 1,
  "bundleName": "Premium T-Shirt Bundle",
  "quantitySold": 2,
  "totalAmount": 1198.00,
  "transactionId": "abc-123-def-456",
  "transactionDate": "2025-10-16T14:30:00Z",
  "stockDeductions": [
    {
      "itemType": "Variant",
      "itemId": 1,
      "itemName": "T-Shirt Red M",
      "quantityDeducted": 4,
      "stockBefore": 50,
      "stockAfter": 46
    },
    {
      "itemType": "Variant",
      "itemId": 5,
      "itemName": "Cap Blue",
      "quantityDeducted": 2,
      "stockBefore": 10,
      "stockAfter": 8
    }
  ],
  "remainingBundleStock": 8
}
```

---

## 🔧 Advanced Configuration (Optional)

### Enable XML Comments

To include XML documentation comments in Swagger:

1. **Enable XML Documentation in Project File:**

Edit `FlowAccount.API.csproj`:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

2. **Add XML Comments to Controllers:**

```csharp
/// <summary>
/// Get all products
/// </summary>
/// <returns>List of all products</returns>
/// <response code="200">Returns the list of products</response>
/// <response code="500">If an error occurs</response>
[HttpGet]
[ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
[ProducesResponseType(500)]
public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
{
    // ...
}
```

3. **Update Program.cs:**

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ... existing configuration ...
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
```

### Add Authorization (Future)

When you add authentication:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ... existing configuration ...
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

### Versioning

For API versioning:

```csharp
// Install: Asp.Versioning.Mvc.ApiExplorer

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FlowAccount API", Version = "v1" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "FlowAccount API", Version = "v2" });
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2");
});
```

---

## 🚀 Production Configuration

### Disable Swagger in Production

Swagger is currently configured to run **only in Development** mode:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(/* ... */);
}
```

### Enable in Production (Optional)

If you need Swagger in production (with authentication):

```csharp
// Always enable Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FlowAccount API v1");
    options.RoutePrefix = "api-docs"; // Access at /api-docs
});
```

**Security Recommendation:** Add authentication middleware before Swagger in production.

---

## 📊 Integration with Other Tools

### Postman

1. Open Swagger UI at http://localhost:5159
2. Download OpenAPI spec: http://localhost:5159/swagger/v1/swagger.json
3. Import to Postman: **Import → Link → Paste URL**

### ReDoc (Alternative UI)

```bash
dotnet add package Swashbuckle.AspNetCore.ReDoc
```

```csharp
app.UseReDoc(options =>
{
    options.DocumentTitle = "FlowAccount API Documentation";
    options.SpecUrl = "/swagger/v1/swagger.json";
});
```

### Swagger Codegen

Generate client SDKs from OpenAPI spec:

```bash
# Download spec
curl http://localhost:5159/swagger/v1/swagger.json -o flowaccount-api.json

# Generate client (requires swagger-codegen-cli)
swagger-codegen generate -i flowaccount-api.json -l csharp -o ./client
```

---

## ✅ Testing Checklist

- [x] Swagger UI loads at http://localhost:5159
- [ ] All 14 endpoints visible in UI
- [ ] "Try it out" works for GET endpoints
- [ ] "Try it out" works for POST endpoints
- [ ] Request/response examples are clear
- [ ] Schema documentation is complete
- [ ] Request duration is displayed
- [ ] API responses match documentation

---

## 🐛 Troubleshooting

### Swagger UI doesn't load

**Check:**
1. API is running: `dotnet run`
2. Environment is Development: Check `ASPNETCORE_ENVIRONMENT`
3. Port is correct: Default is http://localhost:5159

### Endpoints missing in Swagger

**Check:**
1. Controllers have `[ApiController]` attribute
2. Actions have HTTP method attributes (`[HttpGet]`, `[HttpPost]`, etc.)
3. Controllers are registered: `builder.Services.AddControllers()`

### Schema conflicts

**Solution:** Already configured with `CustomSchemaIds(type => type.FullName)`

---

## 📚 Additional Resources

- **Swashbuckle Documentation:** https://github.com/domaindrivendev/Swashbuckle.AspNetCore
- **OpenAPI Specification:** https://swagger.io/specification/
- **Swagger UI:** https://swagger.io/tools/swagger-ui/

---

**Status:** ✅ Swagger Setup Complete  
**Access URL:** http://localhost:5159  
**Configuration:** Development Only  
**Features:** Interactive API testing, Schema docs, Request duration tracking
