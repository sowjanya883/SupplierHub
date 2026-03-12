using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Constants.Enum;
using SupplierHub.Models;


namespace SupplierHub.Config.Configurations
{
	public class RFxLineConfiguration : IEntityTypeConfiguration<RFxLine>
	{
		public void Configure(EntityTypeBuilder<RFxLine> builder)
		{

			builder.Property(rl => rl.UoM)
				   .HasDefaultValue(UOM.EA);

			
			builder.Property(rl => rl.IsDeleted)
				   .HasDefaultValue(false);

			builder.HasQueryFilter(rl => rl.IsDeleted == false);

			
			builder.HasOne(rl => rl.RFxEvent)
				   .WithMany(e => e.RFxLines)
				   .HasForeignKey(rl => rl.RFxID)
				   .OnDelete(DeleteBehavior.Cascade);
			
			builder.HasOne(rl => rl.Item)
				   .WithMany(e=>e.RFxLines) 
				   .HasForeignKey(rl => rl.ItemID)
				   .OnDelete(DeleteBehavior.Restrict);
			
			builder.HasMany(rl => rl.BidLines)
				   .WithOne(bl => bl.RFxLine)
				   .HasForeignKey(bl => bl.RFxLineID)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
