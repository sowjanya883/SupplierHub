using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Constants.Enum;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	// Invoice
	public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
	{
		public void Configure(EntityTypeBuilder<Invoice> builder)
		{
			builder.HasKey(x => x.InvoiceID);
			builder.Property(x => x.InvoiceID).ValueGeneratedOnAdd();

			builder.Property(x => x.InvoiceNo).HasMaxLength(100);
			builder.Property(x => x.InvoiceDate).HasColumnType("date");
			builder.Property(x => x.Currency).HasMaxLength(10);
			builder.Property(x => x.TotalAmount).HasPrecision(18, 2);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("SUBMITTED");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PoID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// InvoiceLine
	public class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
	{
		public void Configure(EntityTypeBuilder<InvoiceLine> builder)
		{
			builder.HasKey(x => x.InvLineID);
			builder.Property(x => x.InvLineID).ValueGeneratedOnAdd();

			builder.Property(x => x.Qty).HasPrecision(18, 3);
			builder.Property(x => x.UnitPrice).HasPrecision(18, 4);
			builder.Property(x => x.LineTotal).HasPrecision(18, 2);
			builder.Property(x => x.MatchStatus).HasMaxLength(20);

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Invoice>().WithMany().HasForeignKey(x => x.InvoiceID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<PoLine>().WithMany().HasForeignKey(x => x.PoLineID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// MatchRef
	public class MatchRefConfiguration : IEntityTypeConfiguration<MatchRef>
	{
		public void Configure(EntityTypeBuilder<MatchRef> builder)
		{
			builder.HasKey(x => x.MatchID);
			builder.Property(x => x.MatchID).ValueGeneratedOnAdd();

			builder.Property(x => x.Result).HasMaxLength(20);
			builder.Property(x => x.Notes).HasMaxLength(500);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Invoice>().WithMany().HasForeignKey(x => x.InvoiceID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PoID)
				   .OnDelete(DeleteBehavior.Restrict);
			builder.HasOne<GrnRef>().WithMany().HasForeignKey(x => x.GrnID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}