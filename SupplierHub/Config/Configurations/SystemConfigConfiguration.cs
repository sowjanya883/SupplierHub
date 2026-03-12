using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;
using SupplierHub.Constants.Enum;
namespace SupplierHub.Config.Configurations
{
	public class SystemConfigConfiguration : IEntityTypeConfiguration<SystemConfig>
	{
		public void Configure(EntityTypeBuilder<SystemConfig> builder)
		{
			builder.HasIndex(c => c.Key)
				   .IsUnique()
				   .HasFilter("[IsDeleted] = 0"); 

			builder.Property(c => c.value)
				   .IsRequired();

			builder.Property(c => c.Scope)
				   .HasDefaultValue(Scope.Global)
				   .HasConversion<string>();
				   

			builder.Property(c => c.UpdatedDate)
				   .HasDefaultValueSql("GETDATE()")
				   .ValueGeneratedOnAddOrUpdate(); 

			
			builder.Property(c => c.IsDeleted)
				   .HasDefaultValue(false);

			builder.HasQueryFilter(c => c.IsDeleted == false);

			
			builder.HasOne(c => c.LastEditor)
				   .WithMany(e=>e.systemConfigs) 
				   .HasForeignKey(c => c.UpdatedBy)
				   .OnDelete(DeleteBehavior.SetNull);
			
		}
	}
}
