using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;

namespace SupplierHub.Config.Configurations
{
	// =========================================================
	// ROLE CONFIGURATION
	// =========================================================
	public class RoleConfiguration : IEntityTypeConfiguration<Role>
	{
		public void Configure(EntityTypeBuilder<Role> builder)
		{
			builder.ToTable("Roles");

			builder.HasKey(x => x.RoleID);
			builder.Property(x => x.RoleID)
				   .ValueGeneratedOnAdd();

			builder.Property(x => x.RoleName)
				   .HasMaxLength(100)
				   .IsRequired();

			builder.Property(x => x.Status)
				   .HasMaxLength(30)
				   .IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false);
		}
	}

	// =========================================================
	// PERMISSION CONFIGURATION
	// =========================================================
	public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
	{
		public void Configure(EntityTypeBuilder<Permission> builder)
		{
			builder.ToTable("Permissions");

			builder.HasKey(x => x.PermissionID);
			builder.Property(x => x.PermissionID)
				   .ValueGeneratedOnAdd();

			builder.Property(x => x.Code)
				   .HasMaxLength(120)
				   .IsRequired();

			builder.Property(x => x.PermissionName)
				   .HasMaxLength(150)
				   .IsRequired();

			builder.Property(x => x.Status)
				   .HasMaxLength(30)
				   .IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false);
		}
	}

	// =========================================================
	// ROLE–PERMISSION (JOIN TABLE)
	// =========================================================
	public class RolepermissionConfiguration : IEntityTypeConfiguration<RolePermission>
	{
		public void Configure(EntityTypeBuilder<RolePermission> builder)
		{
			builder.ToTable("Rolepermissions");

			// Composite Primary Key
			builder.HasKey(x => new { x.RoleID, x.PermissionID });

			builder.Property(x => x.Status)
				   .HasMaxLength(30)
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false);

			// Relationships (no navigation properties used)
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

	// =========================================================
	// USER CONFIGURATION
	// =========================================================
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("Users");

			builder.HasKey(x => x.UserID);
			builder.Property(x => x.UserID)
				   .ValueGeneratedOnAdd();

			builder.Property(x => x.UserName)
				   .HasMaxLength(150)
				   .IsRequired();

			builder.Property(x => x.Email)
				   .HasMaxLength(150)
				   .IsRequired();

			builder.Property(x => x.Phone)
				   .HasMaxLength(30);

			builder.Property(x => x.PasswordHash)
				   .HasMaxLength(255);

			builder.Property(x => x.Status)
				   .HasMaxLength(30)
				   .IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false);

			// Optional Organization FK
			builder.HasOne<Organization>()
				   .WithMany()
				   .HasForeignKey(x => x.OrgID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// =========================================================
	// USER–ROLE (JOIN TABLE) ✅ FINAL, CORRECT
	// =========================================================
	public class UserroleConfiguration : IEntityTypeConfiguration<UserRole>
		{
			public void Configure(EntityTypeBuilder<UserRole> builder)
			{
				// ✅ Composite Primary Key
				builder.HasKey(ur => new { ur.UserID, ur.RoleID });

				// ✅ Properties
				builder.Property(ur => ur.Status)
					   .HasMaxLength(30)
					   .IsRequired()
					   .HasDefaultValue("ACTIVE");

				builder.Property(ur => ur.CreatedOn)
					   .HasDefaultValueSql("CURRENT_TIMESTAMP")
					   .IsRequired();

				builder.Property(ur => ur.UpdatedOn)
					   .HasDefaultValueSql("CURRENT_TIMESTAMP")
					   .ValueGeneratedOnAddOrUpdate()
					   .IsRequired();

				builder.Property(ur => ur.IsDeleted)
					   .HasDefaultValue(false)
					   .IsRequired();

				// ✅ Relationship → User
				builder.HasOne(ur => ur.User)
					   .WithMany(u => u.UserRoles)
					   .HasForeignKey(ur => ur.UserID)
					   .OnDelete(DeleteBehavior.Restrict);

				// ✅ Relationship → Role
				builder.HasOne(ur => ur.Role)
					   .WithMany(r => r.UserRoles)
					   .HasForeignKey(ur => ur.RoleID)
					   .OnDelete(DeleteBehavior.Restrict);

				builder.ToTable("UserRoles");
			}
		}

	// =========================================================
	// AUDIT LOG CONFIGURATION
	// =========================================================
	public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
	{
		public void Configure(EntityTypeBuilder<AuditLog> builder)
		{
			builder.ToTable("AuditLogs");

			builder.HasKey(x => x.AuditID);
			builder.Property(x => x.AuditID)
				   .ValueGeneratedOnAdd();

			builder.Property(x => x.Action)
				   .HasMaxLength(100)
				   .IsRequired();

			builder.Property(x => x.Resource)
				   .HasMaxLength(200)
				   .IsRequired();

			builder.Property(x => x.Timestamp)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");

			builder.Property(x => x.Status)
				   .HasMaxLength(30)
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate();

			builder.Property(x => x.IsDeleted)
				   .HasDefaultValue(false);

			builder.HasOne<User>()
				   .WithMany()
				   .HasForeignKey(x => x.UserID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}
