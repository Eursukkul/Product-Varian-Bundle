using FlowAccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowAccount.Infrastructure.Data.Configurations;

public class VariantOptionValueConfiguration : IEntityTypeConfiguration<VariantOptionValue>
{
    public void Configure(EntityTypeBuilder<VariantOptionValue> builder)
    {
        builder.ToTable("VariantOptionValues");

        builder.HasKey(vov => vov.Id);

        builder.Property(vov => vov.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vov => vov.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(vov => vov.VariantOption)
            .WithMany(vo => vo.Values)
            .HasForeignKey(vov => vov.VariantOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(vov => vov.ProductVariantAttributes)
            .WithOne(pva => pva.VariantOptionValue)
            .HasForeignKey(pva => pva.VariantOptionValueId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(vov => new { vov.VariantOptionId, vov.DisplayOrder })
            .HasDatabaseName("IX_VariantOptionValues_VariantOptionId_DisplayOrder");
    }
}
