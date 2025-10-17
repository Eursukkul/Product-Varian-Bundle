using FlowAccount.Domain.Interfaces;
using FlowAccount.Infrastructure.Data;
using FlowAccount.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using FlowAccount.Application.Services.Interfaces;
using FlowAccount.Application.Services;
using Serilog;
using Serilog.Events;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

// Configure Serilog
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

    // Configure Swagger/OpenAPI - Always enabled for review and demo purposes
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "FlowAccount – Product Variant & Bundle API",
            Version = "v1.0.0",
            Description = @"**Enterprise-grade Inventory Management System API**

This API provides comprehensive product variant and bundle management capabilities:

**Core Features:**
- 🚀 **Batch Operations**: Generate up to 250 product variants in a single operation
- 📊 **Stock Logic**: Intelligent bottleneck detection for bundle inventory management
- 💾 **Transaction Management**: Atomic operations with automatic rollback on failure
- 🔍 **Advanced Querying**: Filter, search, and paginate across all entities

**Main Endpoints:**
- **Products**: CRUD operations for product masters and variants
- **Bundles**: Create and manage product bundles with stock calculations
- **Stock**: Warehouse inventory management and tracking

**Performance:**
- Tested with 250 concurrent variant generation (2,043ms)
- Average 8.2ms per variant
- 100% test coverage on core business logic",
            Contact = new OpenApiContact
            {
                Name = "FlowAccount Development Team",
                Email = "support@flowaccount.com",
                Url = new Uri("https://github.com/Eursukkul/Product-Varian-Bundle")
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        });

        // Enable annotations for enhanced documentation
        options.EnableAnnotations();

        // Enable example filters for request/response examples
        options.ExampleFilters();

        // Use full names for schema IDs to avoid conflicts
        options.CustomSchemaIds(type => type.FullName);

        // Include XML comments if available
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }

        // Add operation filters for consistent API responses
        options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

    // Register Swagger example providers
    builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

    // Add DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("FlowAccount.Infrastructure")));

    // Add Repositories
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IVariantRepository, VariantRepository>();
    builder.Services.AddScoped<IBundleRepository, BundleRepository>();
    builder.Services.AddScoped<IStockRepository, StockRepository>();

    // Add AutoMapper
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // Add Application Services
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IBundleService, BundleService>();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Add Controllers
    builder.Services.AddControllers();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    // Swagger is always enabled for review and demo purposes
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FlowAccount – Product Variant & Bundle API v1.0.0");
        options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        options.DocumentTitle = "FlowAccount API – Product Variant & Bundle Documentation";
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
        options.EnableValidator();
        options.DisplayOperationId();
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });

    app.UseHttpsRedirection();

    app.UseCors("AllowAll");

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

    app.UseAuthorization();

    app.MapControllers();

    // Health check endpoint
    app.MapGet("/health", () => new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow,
        application = "FlowAccount API",
        version = "1.0.0"
    })
    .WithName("HealthCheck")
    .WithTags("Health");

    Log.Information("FlowAccount API started successfully");

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