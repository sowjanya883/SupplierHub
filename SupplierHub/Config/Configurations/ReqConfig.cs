using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	// Requisition
	public class RequisitionConfiguration : IEntityTypeConfiguration<Requisition>
	{
		public void Configure(EntityTypeBuilder<Requisition> builder)
		{
			builder.HasKey(x => x.PrID);
			builder.Property(x => x.PrID).ValueGeneratedOnAdd();

			builder.Property(x => x.CostCenter).HasMaxLength(50);
			builder.Property(x => x.Justification).HasMaxLength(500);
			builder.Property(x => x.RequestedDate).HasColumnType("date");
			builder.Property(x => x.NeededByDate).HasColumnType("date");

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("DRAFT");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<User>().WithMany().HasForeignKey(x => x.RequesterID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrgID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}

	// PrLine
	public class PRLineConfiguration : IEntityTypeConfiguration<PrLine>
	{
		public void Configure(EntityTypeBuilder<PrLine> builder)
		{
			builder.HasKey(x => x.PrLineID);
			builder.Property(x => x.PrLineID).ValueGeneratedOnAdd();

			builder.Property(x => x.Description).HasMaxLength(500);
			builder.Property(x => x.Qty).HasPrecision(18, 3);
			builder.Property(x => x.Uom).HasMaxLength(30);
			builder.Property(x => x.TargetPrice).HasPrecision(18, 2);
			builder.Property(x => x.Currency).HasMaxLength(10);
			builder.Property(x => x.Notes).HasMaxLength(500);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Requisition>().WithMany().HasForeignKey(x => x.PrID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemID)
				   .OnDelete(DeleteBehavior.Restrict);
			builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierPreferredID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// ApprovalStep
	public class ApprovalStepConfiguration : IEntityTypeConfiguration<ApprovalStep>
	{
		public void Configure(EntityTypeBuilder<ApprovalStep> builder)
		{
			builder.HasKey(x => x.StepID);
			builder.Property(x => x.StepID).ValueGeneratedOnAdd();

			builder.Property(x => x.Decision).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("PENDING");
			builder.Property(x => x.DecisionDate);
			builder.Property(x => x.Remarks).HasMaxLength(500);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Requisition>().WithMany().HasForeignKey(x => x.PrID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<User>().WithMany().HasForeignKey(x => x.ApproverID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}
}