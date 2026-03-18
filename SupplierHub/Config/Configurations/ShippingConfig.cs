using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Constants.Enum;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	// Shipment
	public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
	{
		public void Configure(EntityTypeBuilder<Shipment> builder)
		{
			builder.HasKey(x => x.ShipmentID);
			builder.Property(x => x.ShipmentID).ValueGeneratedOnAdd();

			builder.Property(x => x.ShipDate).HasColumnType("date");
			builder.Property(x => x.Carrier).HasMaxLength(100);
			builder.Property(x => x.TrackingNo).HasMaxLength(100);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("PLANNED");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PoID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}

	// Asn
	public class ASNConfiguration : IEntityTypeConfiguration<Asn>
	{
		public void Configure(EntityTypeBuilder<Asn> builder)
		{
			builder.HasKey(x => x.AsnID);
			builder.Property(x => x.AsnID).ValueGeneratedOnAdd();

			builder.Property(x => x.AsnNo).HasMaxLength(100);
			builder.Property(x => x.CreatedDate);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("OPEN");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Shipment>().WithMany().HasForeignKey(x => x.ShipmentID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}

	// AsnItem
	public class ASNItemConfiguration : IEntityTypeConfiguration<AsnItem>
	{
		public void Configure(EntityTypeBuilder<AsnItem> builder)
		{
			builder.HasKey(x => x.AsnItemID);
			builder.Property(x => x.AsnItemID).ValueGeneratedOnAdd();

			builder.Property(x => x.ShippedQty).HasPrecision(18, 3);
			builder.Property(x => x.LotBatch).HasMaxLength(100);
			builder.Property(x => x.Notes).HasMaxLength(500);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Asn>().WithMany().HasForeignKey(x => x.AsnID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();

			builder.HasOne<PoLine>().WithMany().HasForeignKey(x => x.PoLineID)

				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}

	// DeliverySlot (FK points to Site(SiteID) in Excel; Site entity not in current model set)
	public class DeliverySlotConfiguration : IEntityTypeConfiguration<DeliverySlot>
	{
		public void Configure(EntityTypeBuilder<DeliverySlot> builder)
		{
			builder.HasKey(x => x.SlotID);
			builder.Property(x => x.SlotID).ValueGeneratedOnAdd();

			builder.Property(x => x.Date).HasColumnType("date").IsRequired();
			builder.Property(x => x.StartTime).HasColumnType("time").IsRequired();
			builder.Property(x => x.EndTime).HasColumnType("time").IsRequired();
			builder.Property(x => x.Capacity);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("OPEN");

			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			// Not adding FK to Site due to missing Site entity; add later if/when Site is modeled.
		}
	}
}