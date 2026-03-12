using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Constants.Enum;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	public class BidConfiguration : IEntityTypeConfiguration<Bid>
	{
		public void Configure(EntityTypeBuilder<Bid> builder)
		{
			
			builder.HasKey(b => b.BidID);

			builder.Property(b => b.Status)
				   .HasDefaultValue(BidStatus.Submitted)
				   .HasConversion<string>(); 

			
			builder.Property(b => b.IsDeleted)
				   .HasDefaultValue(false);

			builder.HasQueryFilter(b => b.IsDeleted == false);

			builder.Property(b => b.BidDate)
				   .HasDefaultValueSql("GETDATE()"); 

		
			builder.HasOne(b => b.RFxEvent)
				   .WithMany(e => e.Bids)
				   .HasForeignKey(b => b.RFxID)
				   .OnDelete(DeleteBehavior.Cascade);
			
			builder.HasOne(b => b.Supplier)
				   .WithMany(s => s.Bids)
				   .HasForeignKey(b => b.SupplierId)
				   .OnDelete(DeleteBehavior.SetNull);
			
			builder.HasMany(b => b.BidLines)
				   .WithOne(bl => bl.Bid)
				   .HasForeignKey(bl => bl.BidID)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
