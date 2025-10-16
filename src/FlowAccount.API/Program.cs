using FlowAccount.Domain.Interfaces;
using FlowAccount.Infrastructure.Data;
using FlowAccount.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using FlowAccount.Application.Services.Interfaces;
using FlowAccount.Application.Services;
using Serilog;
using Serilog.Events;

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

    // Configure Swagger/OpenAPI
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

        // Add operation filters for better documentation
        options.CustomSchemaIds(type => type.FullName);
    });

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

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FlowAccount API v1");
            options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            options.DocumentTitle = "FlowAccount API Documentation";
            options.DisplayRequestDuration();
        });
    }

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