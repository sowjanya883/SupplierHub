using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;



//  IdentityConfig.cs (Role, Permission, RolePermission, User, UserRole, AuditLog)

namespace SupplierHub.Config.Configurations
{
	// Role
	public class RoleConfiguration : IEntityTypeConfiguration<Role>
	{
		public void Configure(EntityTypeBuilder<Role> builder)
		{
			builder.HasKey(x => x.RoleID);
			builder.Property(x => x.RoleID).ValueGeneratedOnAdd();

			builder.Property(x => x.RoleName).HasMaxLength(100).IsRequired();

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false)
				   .IsRequired();
		}
	}

	// Permission
	public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
	{
		public void Configure(EntityTypeBuilder<Permission> builder)
		{
			builder.HasKey(x => x.PermissionID);
			builder.Property(x => x.PermissionID).ValueGeneratedOnAdd();

			builder.Property(x => x.Code).HasMaxLength(120).IsRequired();
			builder.Property(x => x.PermissionName).HasMaxLength(150).IsRequired();

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false)
				   .IsRequired();
		}
	}

	// RolePermission (composite key)
	public class RolepermissionConfiguration : IEntityTypeConfiguration<RolePermission>
	{
		public void Configure(EntityTypeBuilder<RolePermission> builder)
		{
			builder.HasKey(x => new { x.RoleID, x.PermissionID });

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false)
				   .IsRequired();

			builder.HasOne<Role>()
				   .WithMany()
				   .HasForeignKey(x => x.RoleID)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne<Permission>()
				   .WithMany()
				   .HasForeignKey(x => x.PermissionID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// User
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(x => x.UserID);
			builder.Property(x => x.UserID).ValueGeneratedOnAdd();

			builder.Property(x => x.UserName).HasMaxLength(150).IsRequired();
			builder.Property(x => x.Email).HasMaxLength(150).IsRequired();
			builder.Property(x => x.Phone).HasMaxLength(30);
			builder.Property(x => x.PasswordHash).HasMaxLength(255);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false)
				   .IsRequired();

			builder.HasOne<Organization>()
				   .WithMany()
				   .HasForeignKey(x => x.OrgID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// UserRole (composite key)
	public class UserroleConfiguration : IEntityTypeConfiguration<UserRole>
	{
		public void Configure(EntityTypeBuilder<UserRole> builder)
		{
			builder.HasKey(x => new { x.UserID, x.RoleID });

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false)
				   .IsRequired();

			builder.HasOne<User>()
				   .WithMany()
				   .HasForeignKey(x => x.UserID)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne<Role>()
				   .WithMany()
				   .HasForeignKey(x => x.RoleID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// AuditLog
	public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
	{
		public void Configure(EntityTypeBuilder<AuditLog> builder)
		{
			builder.HasKey(x => x.AuditID);
			builder.Property(x => x.AuditID).ValueGeneratedOnAdd();

			builder.Property(x => x.Action).HasMaxLength(100).IsRequired();
			builder.Property(x => x.Resource).HasMaxLength(200).IsRequired();

			builder.Property(x => x.Timestamp)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false)
				   .IsRequired();

			builder.HasOne<User>()
				   .WithMany()
				   .HasForeignKey(x => x.UserID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}