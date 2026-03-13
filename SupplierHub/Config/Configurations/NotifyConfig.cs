using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
	{
		public void Configure(EntityTypeBuilder<Notification> builder)
		{
			builder.HasKey(x => x.NotificationID);
			builder.Property(x => x.NotificationID).ValueGeneratedOnAdd();

			builder.Property(x => x.Message).HasMaxLength(500).IsRequired();
			builder.Property(x => x.Category).HasMaxLength(30);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("UNREAD");

			builder.Property(x => x.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserID)
				   .OnDelete(DeleteBehavior.Restrict).IsRequired();
		}
	}
}