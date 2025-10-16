using FlowAccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowAccount.Infrastructure.Data.Configurations;

public class ProductMasterConfiguration : IEntityTypeConfiguration<ProductMaster>
{
    public void Configure(EntityTypeBuilder<ProductMaster> builder)
    {
        builder.ToTable("ProductMasters");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.VariantOptions)
            .WithOne(vo => vo.ProductMaster)
            .HasForeignKey(vo => vo.ProductMasterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductVariants)
            .WithOne(pv => pv.ProductMaster)
            .HasForeignKey(pv => pv.ProductMasterId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_ProductMasters_CategoryId");

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_ProductMasters_Name");

        builder.HasIndex(p => new { p.IsActive, p.CategoryId })
            .HasDatabaseName("IX_ProductMasters_IsActive_CategoryId");
    }
}
