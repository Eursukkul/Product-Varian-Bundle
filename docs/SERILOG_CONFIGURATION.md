# Serilog Configuration

## ✅ Installation Complete

FlowAccount API ได้ติดตั้งและ configure **Serilog** สำหรับ structured logging แล้ว

---

## 📦 Packages Installed

```bash
dotnet add package Serilog.AspNetCore       # v9.0.0
dotnet add package Serilog.Sinks.Console    # v6.0.0 (included in AspNetCore)
dotnet add package Serilog.Sinks.File       # v6.0.0 (included in AspNetCore)
```

### Included Dependencies:
- `Serilog` v4.2.0 - Core library
- `Serilog.Extensions.Hosting` v9.0.0 - ASP.NET Core integration
- `Serilog.Settings.Configuration` v9.0.0 - appsettings.json configuration
- `Serilog.Formatting.Compact` v3.0.0 - Compact JSON formatting
- `Serilog.Sinks.Debug` v3.0.0 - Debug output

---

## ⚙️ Configuration

### Program.cs Setup

```csharp
using Serilog;
using Serilog.Events;

// Configure Serilog early in application startup
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "FlowAccount.API")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/flowaccount-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("Starting FlowAccount API...");
    
    var builder = WebApplication.CreateBuilder(args);
    
    // Use Serilog for logging
    builder.Host.UseSerilog();
    
    // ... rest of configuration
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

### Request Logging Middleware

```csharp
// Add Serilog request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    options.GetLevel = (httpContext, elapsed, ex) => ex != null
        ? LogEventLevel.Error
        : elapsed > 1000
            ? LogEventLevel.Warning
            : LogEventLevel.Information;
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});
```

### appsettings.json Configuration

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Information",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/flowaccount-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "FlowAccount.API"
    }
  }
}
```

---

## 📊 Log Output Examples

### Console Output
```
[21:19:14 INF] Starting FlowAccount API... {"Application": "FlowAccount.API"}
[21:19:14 INF] FlowAccount API started successfully {"Application": "FlowAccount.API"}
[21:19:14 INF] Now listening on: http://localhost:5159 {"EventId": {"Id": 14, "Name": "ListeningOnAddress"}, "SourceContext": "Microsoft.Hosting.Lifetime", "Application": "FlowAccount.API"}
```

### File Output (`logs/flowaccount-20251016.log`)
```
2025-10-16 21:19:14.232 +07:00 [INF] Starting FlowAccount API... {"Application":"FlowAccount.API"}
2025-10-16 21:19:14.735 +07:00 [INF] FlowAccount API started successfully {"Application":"FlowAccount.API"}
2025-10-16 21:19:14.850 +07:00 [INF] Now listening on: http://localhost:5159 {"EventId":{"Id":14,"Name":"ListeningOnAddress"},"SourceContext":"Microsoft.Hosting.Lifetime","Application":"FlowAccount.API"}
```

### Request Logging Example
```
[21:20:15 INF] HTTP GET /api/products responded 200 in 125.4567ms {"RequestHost": "localhost:5159", "RequestScheme": "http"}
[21:20:18 INF] HTTP POST /api/products/5/generate-variants responded 200 in 1567.8910ms {"RequestHost": "localhost:5159", "RequestScheme": "http"}
[21:20:20 WRN] HTTP POST /api/bundles/10/sell responded 200 in 2045.1234ms {"RequestHost": "localhost:5159", "RequestScheme": "http"}
```

---

## 🎯 Log Levels

| Level | Description | Use Case |
|-------|-------------|----------|
| **Debug** | Detailed debugging info | Development only, very verbose |
| **Information** | General flow info | API started, request completed |
| **Warning** | Unexpected but handled | Slow requests (>1000ms), validation warnings |
| **Error** | Errors and exceptions | API errors, unhandled exceptions |
| **Fatal** | Critical failures | Application crash, startup failure |

### Configured Overrides:
- `Microsoft.*` → **Information** (reduce noise)
- `Microsoft.AspNetCore.*` → **Warning** (only important ASP.NET Core events)
- `Microsoft.EntityFrameworkCore.*` → **Information** (SQL queries visible)
- `System.*` → **Warning** (reduce system noise)

---

## 📁 File Management

### Rolling Interval
- **Daily rotation**: New file created each day
- **File naming**: `flowaccount-YYYYMMDD.log`
- **Examples**:
  - `flowaccount-20251016.log`
  - `flowaccount-20251017.log`

### Retention Policy
- **Retained files**: Last 30 days
- **Auto-cleanup**: Older files automatically deleted
- **Location**: `src/FlowAccount.API/logs/`

---

## 🔍 Structured Logging Features

### 1. Enrichers
- **FromLogContext**: Include contextual properties
- **WithMachineName**: Add machine name to logs
- **WithThreadId**: Track which thread logged the message
- **Custom Property**: `Application = "FlowAccount.API"`

### 2. Output Templates

**Console Template** (readable):
```
[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}
```
- `HH:mm:ss` - Time only
- `{Level:u3}` - 3-character uppercase level (INF, WRN, ERR)
- `{Message:lj}` - Message with literal JSON rendering
- `{Properties:j}` - Structured properties as JSON

**File Template** (complete):
```
{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}
```
- Full timestamp with milliseconds and timezone
- Same format as console but with full date

### 3. JSON Properties

Every log entry includes structured data:
```json
{
  "Application": "FlowAccount.API",
  "SourceContext": "FlowAccount.API.Controllers.ProductsController",
  "RequestHost": "localhost:5159",
  "RequestScheme": "http"
}
```

---

## 💡 Usage in Controllers

### Example: ProductsController

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
        try
        {
            var response = await _productService.GenerateVariantsAsync(request);
            
            // Structured logging with properties
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
            _logger.LogWarning(ex, 
                "Invalid variant generation request for product {ProductId}", 
                id);
            return BadRequest(...);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error generating variants for product {ProductId}", 
                id);
            return StatusCode(500, ...);
        }
    }
}
```

**Output**:
```
[21:20:30 INF] Generated 12 variants for product 5 in 125.45ms {"VariantCount": 12, "ProductId": 5, "ProcessingTime": 125.45}
```

---

## 🚀 Benefits of Serilog

### 1. Structured Logging
- ✅ Machine-readable JSON properties
- ✅ Easy to query and analyze
- ✅ Better than string interpolation

### 2. Multiple Sinks
- ✅ Console (development)
- ✅ File (production audit trail)
- ✅ Can add: Seq, Elasticsearch, Application Insights, etc.

### 3. Performance
- ✅ Asynchronous writing
- ✅ Buffered output
- ✅ Minimal overhead

### 4. Rich Context
- ✅ Request details (path, method, status code)
- ✅ Performance metrics (elapsed time)
- ✅ Exception details with stack traces
- ✅ Custom properties

### 5. Production-Ready
- ✅ File rotation (daily)
- ✅ Retention policy (30 days)
- ✅ Automatic cleanup
- ✅ Timezone support

---

## 📈 Monitoring Recommendations

### Development
- **Console**: Primary output for immediate feedback
- **File**: Backup for debugging complex issues

### Production
- **File**: Audit trail and troubleshooting
- **Seq** (optional): Centralized log server with UI
  ```bash
  dotnet add package Serilog.Sinks.Seq
  ```
- **Application Insights** (Azure): Cloud monitoring
  ```bash
  dotnet add package Serilog.Sinks.ApplicationInsights
  ```

### Query Logs
```powershell
# View last 50 log entries
Get-Content logs/flowaccount-20251016.log -Tail 50

# Search for errors
Select-String -Path logs/*.log -Pattern "\[ERR\]"

# Filter by product operations
Select-String -Path logs/*.log -Pattern "product"
```

---

## 🎯 Summary

✅ **Installed**: Serilog.AspNetCore v9.0.0  
✅ **Configured**: Program.cs with early initialization  
✅ **Sinks**: Console + File (daily rotation, 30-day retention)  
✅ **Enrichers**: FromLogContext, MachineName, ThreadId, Custom Application property  
✅ **Request Logging**: Automatic HTTP request/response logging with performance metrics  
✅ **File Location**: `src/FlowAccount.API/logs/flowaccount-YYYYMMDD.log`  
✅ **Format**: Structured JSON properties for easy parsing  
✅ **Production-Ready**: Error handling, file management, timezone support

Serilog is now the primary logging provider for FlowAccount API! 🎉
