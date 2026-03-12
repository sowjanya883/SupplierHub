using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;
using SupplierHub.Constants.Enum;


namespace SupplierHub.Config.Configurations
{
	public class AwardConfiguration : IEntityTypeConfiguration<Award>
	{
		public void Configure(EntityTypeBuilder<Award> builder)
		{
			
			builder.Property(a => a.AwardValue)
				   .HasPrecision(18, 2)
				   .IsRequired();

			builder.Property(a => a.Status)
				   .HasDefaultValue(AwardStatus.Pending)
				   .HasMaxLength(50);

			builder.Property(a => a.AwardDate)
				   .HasDefaultValueSql("GETDATE()");

			builder.Property(a => a.Notes)
				   .HasMaxLength(1000);

			builder.Property(a => a.IsDeleted)
				   .HasDefaultValue(false);

			builder.HasQueryFilter(a => a.IsDeleted == false);

			builder.HasOne(a => a.RFxEvent)
				   .WithOne(e => e.Award)
				   .HasForeignKey<Award>(a => a.RFxID)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(a => a.Supplier)
				   .WithMany(s => s.Awards)
				   .HasForeignKey(a => a.SupplierId)
				   .OnDelete(DeleteBehavior.SetNull);
			
		}
	}
}
