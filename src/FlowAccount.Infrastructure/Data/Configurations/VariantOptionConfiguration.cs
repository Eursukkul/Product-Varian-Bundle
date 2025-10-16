using FlowAccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowAccount.Infrastructure.Data.Configurations;

public class VariantOptionConfiguration : IEntityTypeConfiguration<VariantOption>
{
    public void Configure(EntityTypeBuilder<VariantOption> builder)
    {
        builder.ToTable("VariantOptions");

        builder.HasKey(vo => vo.Id);

        builder.Property(vo => vo.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vo => vo.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(vo => vo.ProductMaster)
            .WithMany(p => p.VariantOptions)
            .HasForeignKey(vo => vo.ProductMasterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(vo => vo.Values)
            .WithOne(vov => vov.VariantOption)
            .HasForeignKey(vov => vov.VariantOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(vo => new { vo.ProductMasterId, vo.DisplayOrder })
            .HasDatabaseName("IX_VariantOptions_ProductMasterId_DisplayOrder");
    }
}
