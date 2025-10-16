using FlowAccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowAccount.Infrastructure.Data.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pv => pv.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pv => pv.Cost)
            .HasPrecision(18, 2);

        builder.Property(pv => pv.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pv => pv.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(pv => pv.ProductMaster)
            .WithMany(p => p.ProductVariants)
            .HasForeignKey(pv => pv.ProductMasterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pv => pv.Attributes)
            .WithOne(pva => pva.ProductVariant)
            .HasForeignKey(pva => pva.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pv => pv.SKU)
            .IsUnique()
            .HasDatabaseName("IX_ProductVariants_SKU");

        builder.HasIndex(pv => new { pv.ProductMasterId, pv.IsActive })
            .HasDatabaseName("IX_ProductVariants_ProductMasterId_IsActive");

        builder.HasIndex(pv => pv.Price)
            .HasDatabaseName("IX_ProductVariants_Price");
    }
}
