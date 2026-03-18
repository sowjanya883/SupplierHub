using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	// GrnRef
	public class GRNRefConfiguration : IEntityTypeConfiguration<GrnRef>
	{
		public void Configure(EntityTypeBuilder<GrnRef> builder)
		{
			builder.HasKey(x => x.GrnID);
			builder.Property(x => x.GrnID).ValueGeneratedOnAdd();

			builder.Property(x => x.ReceivedDate);
			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("OPEN");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PoID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Asn>().WithMany().HasForeignKey(x => x.AsnID)
				   .OnDelete(DeleteBehavior.Restrict);
			builder.HasOne<User>().WithMany().HasForeignKey(x => x.ReceivedBy)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// GrnItemRef
	public class GRNItemRefConfiguration : IEntityTypeConfiguration<GrnItemRef>
	{
		public void Configure(EntityTypeBuilder<GrnItemRef> builder)
		{
			builder.HasKey(x => x.GrnItemID);
			builder.Property(x => x.GrnItemID).ValueGeneratedOnAdd();

			builder.Property(x => x.ReceivedQty).HasPrecision(18, 3);
			builder.Property(x => x.AcceptedQty).HasPrecision(18, 3);
			builder.Property(x => x.RejectedQty).HasPrecision(18, 3);
			builder.Property(x => x.Reason).HasMaxLength(200);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<GrnRef>().WithMany().HasForeignKey(x => x.GrnID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<PoLine>().WithMany().HasForeignKey(x => x.PoLineID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}

	// Inspection
	public class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
	{
		public void Configure(EntityTypeBuilder<Inspection> builder)
		{
			builder.HasKey(x => x.InspID);
			builder.Property(x => x.InspID).ValueGeneratedOnAdd();

			builder.Property(x => x.Result).HasMaxLength(10);
			builder.Property(x => x.InspDate);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<GrnItemRef>().WithMany().HasForeignKey(x => x.GrnItemID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<User>().WithMany().HasForeignKey(x => x.InspectorID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// Ncr
	public class NCRConfiguration : IEntityTypeConfiguration<Ncr>
	{
		public void Configure(EntityTypeBuilder<Ncr> builder)
		{
			builder.HasKey(x => x.NcrID);
			builder.Property(x => x.NcrID).ValueGeneratedOnAdd();

			builder.Property(x => x.DefectType).HasMaxLength(100);
			builder.Property(x => x.Severity).HasMaxLength(20);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("OPEN");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<GrnItemRef>().WithMany().HasForeignKey(x => x.GrnItemID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}
}