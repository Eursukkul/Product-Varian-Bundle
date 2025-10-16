using FlowAccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowAccount.Infrastructure.Data.Configurations;

public class ProductVariantAttributeConfiguration : IEntityTypeConfiguration<ProductVariantAttribute>
{
    public void Configure(EntityTypeBuilder<ProductVariantAttribute> builder)
    {
        builder.ToTable("ProductVariantAttributes");

        builder.HasKey(pva => pva.Id);

        // Relationships
        builder.HasOne(pva => pva.ProductVariant)
            .WithMany(pv => pv.Attributes)
            .HasForeignKey(pva => pva.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pva => pva.VariantOptionValue)
            .WithMany(vov => vov.ProductVariantAttributes)
            .HasForeignKey(pva => pva.VariantOptionValueId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(pva => new { pva.ProductVariantId, pva.VariantOptionValueId })
            .IsUnique()
            .HasDatabaseName("IX_ProductVariantAttributes_ProductVariantId_VariantOptionValueId");

        builder.HasIndex(pva => pva.VariantOptionValueId)
            .HasDatabaseName("IX_ProductVariantAttributes_VariantOptionValueId");
    }
}
