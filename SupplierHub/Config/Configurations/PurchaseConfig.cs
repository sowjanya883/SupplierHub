using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Constants.Enum;
using SupplierHub.Models;


namespace SupplierHub.Config.Configurations
{
	// PurchaseOrder
	public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
	{
		public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
		{
			builder.HasKey(x => x.PoID);
			builder.Property(x => x.PoID).ValueGeneratedOnAdd();

			builder.Property(x => x.PoDate).HasColumnType("date");
			builder.Property(x => x.Currency).HasMaxLength(10);
			builder.Property(x => x.Incoterms).HasMaxLength(50);
			builder.Property(x => x.PaymentTerms).HasMaxLength(100);

			builder.Property(x => x.Status).HasMaxLength(50).IsRequired()
				   .HasDefaultValue("Open");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrgID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}

	// PoLine
	public class PoLineConfiguration : IEntityTypeConfiguration<SupplierHub.Models.PoLine>
	{
		public void Configure(EntityTypeBuilder<SupplierHub.Models.PoLine> builder)
		{
			builder.HasKey(x => x.PoLineID);
			builder.Property(x => x.PoLineID).ValueGeneratedOnAdd();

			builder.Property(x => x.Description).HasMaxLength(500);
			builder.Property(x => x.Qty).HasPrecision(18, 3);
			builder.Property(x => x.Uom).HasMaxLength(30);
			builder.Property(x => x.UnitPrice).HasPrecision(18, 4);
			builder.Property(x => x.LineTotal).HasPrecision(18, 2);
			builder.Property(x => x.DeliveryDate).HasColumnType("date");

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("Active");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PoID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// PoAck
	public class POAckConfiguration : IEntityTypeConfiguration<PoAck>
	{
		public void Configure(EntityTypeBuilder<PoAck> builder)
		{
			builder.HasKey(x => x.PocfmID);
			builder.Property(x => x.PocfmID).ValueGeneratedOnAdd();

			builder.Property(x => x.AcknowledgeDate);
			builder.Property(x => x.Decision).HasMaxLength(30);
			builder.Property(x => x.Counternotes).HasMaxLength(500);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("Active");

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

	// PoRevision
	public class PORevisionConfiguration : IEntityTypeConfiguration<PoRevision>
	{
		public void Configure(EntityTypeBuilder<PoRevision> builder)
		{
			builder.HasKey(x => x.PorevID);
			builder.Property(x => x.PorevID).ValueGeneratedOnAdd();

			builder.Property(x => x.RevisionNo).IsRequired();
			builder.Property(x => x.ChangelogJson);
			builder.Property(x => x.ChangeDate);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("Active");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PoID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<User>().WithMany().HasForeignKey(x => x.ChangedBy)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// ErpExportRef
	public class ErpExportRefConfiguration : IEntityTypeConfiguration<ErpExportRef>
	{
		public void Configure(EntityTypeBuilder<ErpExportRef> builder)
		{
			builder.HasKey(x => x.ErprefID);
			builder.Property(x => x.ErprefID).ValueGeneratedOnAdd();

			builder.Property(x => x.EntityType).HasMaxLength(30).IsRequired();
			builder.Property(x => x.PayloadUri).HasMaxLength(500);
			builder.Property(x => x.CorrelationID).HasMaxLength(100);
			builder.Property(x => x.ExportDate);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("Queued");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();
		}
	}
}