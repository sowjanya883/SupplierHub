using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	// RfxEvent
	public class RfxEventConfiguration : IEntityTypeConfiguration<RfxEvent>
	{
		public void Configure(EntityTypeBuilder<RfxEvent> builder)
		{
			builder.HasKey(x => x.RfxID);
			builder.Property(x => x.RfxID).ValueGeneratedOnAdd();

			builder.Property(x => x.Type).HasMaxLength(10).IsRequired();
			builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("OPEN");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Category>().WithMany().HasForeignKey(x => x.CategoryID)
				   .OnDelete(DeleteBehavior.Restrict);
			builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedBy)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// RfxLine
	public class RfxLineConfiguration : IEntityTypeConfiguration<RfxLine>
	{
		public void Configure(EntityTypeBuilder<RfxLine> builder)
		{
			builder.HasKey(x => x.RfxLineID);
			builder.Property(x => x.RfxLineID).ValueGeneratedOnAdd();

			builder.Property(x => x.Qty).HasPrecision(18, 3);
			builder.Property(x => x.Uom).HasMaxLength(30);
			builder.Property(x => x.TargetPrice).HasPrecision(18, 2);
			builder.Property(x => x.Notes).HasMaxLength(500);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<RfxEvent>().WithMany().HasForeignKey(x => x.RfxID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// RfxInvite
	public class RfxInviteConfiguration : IEntityTypeConfiguration<RfxInvite>
	{
		public void Configure(EntityTypeBuilder<RfxInvite> builder)
		{
			builder.HasKey(x => x.InviteID);
			builder.Property(x => x.InviteID).ValueGeneratedOnAdd();

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("INVITED");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<RfxEvent>().WithMany().HasForeignKey(x => x.RfxID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();

			builder.HasIndex(x => new { x.RfxID, x.SupplierID }).IsUnique();
		}
	}

	// Bid
	public class BidConfiguration : IEntityTypeConfiguration<Bid>
	{
		public void Configure(EntityTypeBuilder<Bid> builder)
		{
			builder.HasKey(x => x.BidID);
			builder.Property(x => x.BidID).ValueGeneratedOnAdd();

			builder.Property(x => x.TotalValue).HasPrecision(18, 2);
			builder.Property(x => x.Currency).HasMaxLength(10);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("SUBMITTED");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasIndex(x => new { x.RfxID, x.SupplierID }).IsUnique();

			builder.HasOne<RfxEvent>().WithMany().HasForeignKey(x => x.RfxID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}

	// BidLine
	public class BidLineConfiguration : IEntityTypeConfiguration<BidLine>
	{
		public void Configure(EntityTypeBuilder<BidLine> builder)
		{
			builder.HasKey(x => x.BidLineID);
			builder.Property(x => x.BidLineID).ValueGeneratedOnAdd();

			builder.Property(x => x.UnitPrice).HasPrecision(18, 4);
			builder.Property(x => x.LeadTimeDays);
			builder.Property(x => x.Currency).HasMaxLength(10);
			builder.Property(x => x.Notes).HasMaxLength(500);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Bid>().WithMany().HasForeignKey(x => x.BidID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<RfxLine>().WithMany().HasForeignKey(x => x.RfxLineID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}

	// Award
	public class AwardConfiguration : IEntityTypeConfiguration<Award>
	{
		public void Configure(EntityTypeBuilder<Award> builder)
		{
			builder.HasKey(x => x.AwardID);
			builder.Property(x => x.AwardID).ValueGeneratedOnAdd();

			builder.Property(x => x.AwardValue).HasPrecision(18, 2);
			builder.Property(x => x.Currency).HasMaxLength(10);
			builder.Property(x => x.Notes).HasMaxLength(500);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<RfxEvent>().WithMany().HasForeignKey(x => x.RfxID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
			builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}
}