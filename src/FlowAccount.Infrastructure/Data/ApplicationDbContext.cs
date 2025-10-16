using FlowAccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FlowAccount.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<ProductMaster> ProductMasters { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<VariantOption> VariantOptions { get; set; }
    public DbSet<VariantOptionValue> VariantOptionValues { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
    public DbSet<Bundle> Bundles { get; set; }
    public DbSet<BundleItem> BundleItems { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Stock> Stocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global Query Filters (Soft Delete support - if needed in future)
        // modelBuilder.Entity<ProductMaster>().HasQueryFilter(p => !p.IsDeleted);
    }
}
