using FlowAccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowAccount.Infrastructure.Data.Configurations;

public class BundleItemConfiguration : IEntityTypeConfiguration<BundleItem>
{
    public void Configure(EntityTypeBuilder<BundleItem> builder)
    {
        builder.ToTable("BundleItems");

        builder.HasKey(bi => bi.Id);

        builder.Property(bi => bi.ItemType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(bi => bi.ItemId)
            .IsRequired();

        builder.Property(bi => bi.Quantity)
            .IsRequired();

        // Relationships
        builder.HasOne(bi => bi.Bundle)
            .WithMany(b => b.BundleItems)
            .HasForeignKey(bi => bi.BundleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(bi => new { bi.BundleId, bi.ItemType, bi.ItemId })
            .IsUnique()
            .HasDatabaseName("IX_BundleItems_BundleId_ItemType_ItemId");

        builder.HasIndex(bi => new { bi.ItemType, bi.ItemId })
            .HasDatabaseName("IX_BundleItems_ItemType_ItemId");

        // Check Constraints (will be added via migration)
        // CHECK (ItemType IN ('Product', 'Variant'))
        // CHECK (Quantity > 0)
    }
}
