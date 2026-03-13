using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	// SystemConfig
	public class SystemConfigConfiguration : IEntityTypeConfiguration<SystemConfig>
	{
		public void Configure(EntityTypeBuilder<SystemConfig> builder)
		{
			builder.HasKey(x => x.ConfigID);
			builder.Property(x => x.ConfigID).ValueGeneratedOnAdd();

			builder.Property(x => x.ConfigKey).HasMaxLength(100).IsRequired();
			builder.Property(x => x.Value).HasMaxLength(1000);
			builder.Property(x => x.Scope).HasMaxLength(20).IsRequired()
				   .HasDefaultValue("Global");

			// Excel marks both ConfigKey and Scope as UNIQUE (individually)
			builder.HasIndex(x => x.ConfigKey).IsUnique();
			builder.HasIndex(x => x.Scope).IsUnique();

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.UpdatedDate);
			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<User>().WithMany().HasForeignKey(x => x.UpdatedBy)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// ApprovalRule
	public class ApprovalRuleConfiguration : IEntityTypeConfiguration<ApprovalRule>
	{
		public void Configure(EntityTypeBuilder<ApprovalRule> builder)
		{
			builder.HasKey(x => x.RuleID);
			builder.Property(x => x.RuleID).ValueGeneratedOnAdd();

			builder.Property(x => x.Scope).HasMaxLength(30).IsRequired();
			builder.Property(x => x.Severity).HasMaxLength(10).IsRequired()
				   .HasDefaultValue("Info");

			builder.Property(x => x.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
			builder.Property(x => x.UpdatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate().IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();
		}
	}
}