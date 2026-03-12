using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	public class BidLineConfiguration : IEntityTypeConfiguration<BidLine>
	{
		public void Configure(EntityTypeBuilder<BidLine> builder) {
			builder.HasKey(e => e.BidLineID);
			builder.Property(bl => bl.UnitPrice)
				   .IsRequired();

			builder.Property(bl => bl.LeadTimeDays)
				   .IsRequired();

			builder.Property(bl => bl.Notes)
				   .HasMaxLength(500); 

			
			builder.Property(bl => bl.IsDeleted)
				   .HasDefaultValue(false);

			builder.HasQueryFilter(bl => bl.IsDeleted == false);

			
			builder.HasOne(bl => bl.Bid)
				   .WithMany(b => b.BidLines)
				   .HasForeignKey(bl => bl.BidID)
				   .OnDelete(DeleteBehavior.Cascade);
			
			builder.HasOne(bl => bl.RFxLine)
				   .WithMany(rl => rl.BidLines)
				   .HasForeignKey(bl => bl.RFxLineID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}
