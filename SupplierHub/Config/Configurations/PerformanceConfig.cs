using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	// SupplierKpi
	public class SupplierKPIConfiguration : IEntityTypeConfiguration<SupplierKpi>
	{
		public void Configure(EntityTypeBuilder<SupplierKpi> builder)
		{
			builder.HasKey(x => x.KpiID);
			builder.Property(x => x.KpiID).ValueGeneratedOnAdd();

			builder.Property(x => x.Period).HasMaxLength(20).IsRequired();
			builder.Property(x => x.NcrRatePpm).HasPrecision(12, 6);
			builder.Property(x => x.AvgAckTimeHrs).HasPrecision(10, 2);
			builder.Property(x => x.PriceAdherencePct).HasPrecision(5, 2);
			builder.Property(x => x.Score).HasPrecision(6, 2);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			// Composite uniqueness: SupplierID + Period
			builder.HasIndex(x => new { x.SupplierID, x.Period }).IsUnique();

			builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}

	// Scorecard
	public class ScorecardConfiguration : IEntityTypeConfiguration<Scorecard>
	{
		public void Configure(EntityTypeBuilder<Scorecard> builder)
		{
			builder.HasKey(x => x.ScorecardID);
			builder.Property(x => x.ScorecardID).ValueGeneratedOnAdd();

			builder.Property(x => x.Period).HasMaxLength(20).IsRequired();
			builder.Property(x => x.Rank);
			builder.Property(x => x.Notes).HasMaxLength(500);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			// Composite uniqueness: SupplierID + Period
			builder.HasIndex(x => new { x.SupplierID, x.Period }).IsUnique();

			builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}
}