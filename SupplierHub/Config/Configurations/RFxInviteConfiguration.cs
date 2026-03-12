using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Constants.Enum;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	public class RFxInviteConfiguration : IEntityTypeConfiguration<RFxInvite>
	{
		public void Configure(EntityTypeBuilder<RFxInvite> builder)
		{
			builder.HasKey(i => i.InviteID);

			builder.Property(i => i.Status)
				   .HasDefaultValue(InviteStatus.Invited) 
				   .HasConversion<string>()    
				   .HasMaxLength(20);

			builder.Property(i => i.InvitedDate)
				   .HasDefaultValueSql("GETDATE()");

			
			builder.Property(i => i.IsDeleted)
				   .HasDefaultValue(false);

			builder.HasQueryFilter(i => i.IsDeleted == false);

			
			builder.HasOne(i => i.RFxEvent)
				   .WithMany(e => e.RFxInvites)
				   .HasForeignKey(i => i.RFxID)
				   .OnDelete(DeleteBehavior.Cascade);
			

			
			builder.HasOne(i => i.Supplier)
				   .WithMany(s => s.RFxInvites)
				   .HasForeignKey(i => i.SupplierId)
				   .OnDelete(DeleteBehavior.SetNull);
		}
	}
}
