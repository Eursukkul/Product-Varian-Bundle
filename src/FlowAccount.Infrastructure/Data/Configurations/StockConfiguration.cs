using FlowAccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowAccount.Infrastructure.Data.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ItemType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.ItemId)
            .IsRequired();

        builder.Property(s => s.Quantity)
            .IsRequired();

        builder.Property(s => s.LastUpdated)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(s => s.Warehouse)
            .WithMany(w => w.Stocks)
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes (CRITICAL for Bundle Stock Calculation!)
        builder.HasIndex(s => new { s.WarehouseId, s.ItemType, s.ItemId })
            .IsUnique()
            .HasDatabaseName("IX_Stocks_WarehouseId_ItemType_ItemId");

        builder.HasIndex(s => new { s.ItemType, s.ItemId, s.WarehouseId })
            .HasDatabaseName("IX_Stocks_ItemType_ItemId_WarehouseId");

        // Check Constraints (will be added via migration)
        // CHECK (ItemType IN ('Product', 'Variant'))
    }
}
