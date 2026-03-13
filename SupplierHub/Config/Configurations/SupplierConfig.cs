using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;

// SupplierConfig.cs (Organization, Supplier, SupplierContact, ComplianceDoc, SupplierRisk)

namespace SupplierHub.Config.Configurations
{
	// Organization
	public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
	{
		public void Configure(EntityTypeBuilder<Organization> builder)
		{
			builder.HasKey(x => x.OrgID);
			builder.Property(x => x.OrgID).ValueGeneratedOnAdd();

			builder.Property(x => x.OrganizationName).HasMaxLength(200).IsRequired();
			builder.Property(x => x.TaxID).HasMaxLength(50);
			builder.Property(x => x.AddressJson); // JSON string storage

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

	// Supplier
	public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
	{
		public void Configure(EntityTypeBuilder<Supplier> builder)
		{
			builder.HasKey(x => x.SupplierID);
			builder.Property(x => x.SupplierID).ValueGeneratedOnAdd();

			builder.Property(x => x.LegalName).HasMaxLength(200).IsRequired();
			builder.Property(x => x.DunsOrRegNo).HasMaxLength(50);
			builder.Property(x => x.TaxID).HasMaxLength(50);
			builder.Property(x => x.BankInfoJson); // JSON string storage
			builder.Property(x => x.PrimaryContactID);

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

	// SupplierContact
	public class SupplierContactConfiguration : IEntityTypeConfiguration<SupplierContact>
	{
		public void Configure(EntityTypeBuilder<SupplierContact> builder)
		{
			builder.HasKey(x => x.ContactID);
			builder.Property(x => x.ContactID).ValueGeneratedOnAdd();

			builder.Property(x => x.SupplierName).HasMaxLength(150).IsRequired();
			builder.Property(x => x.Email).HasMaxLength(150);
			builder.Property(x => x.Phone).HasMaxLength(30);
			builder.Property(x => x.Role).HasMaxLength(100);

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

			builder.HasOne<Supplier>()
				   .WithMany()
				   .HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict)
				   .IsRequired();
		}
	}

	// ComplianceDoc
	public class ComplianceDocConfiguration : IEntityTypeConfiguration<ComplianceDoc>
	{
		public void Configure(EntityTypeBuilder<ComplianceDoc> builder)
		{
			builder.HasKey(x => x.DocID);
			builder.Property(x => x.DocID).ValueGeneratedOnAdd();

			builder.Property(x => x.DocType).HasMaxLength(50).IsRequired();
			builder.Property(x => x.FileUri).HasMaxLength(500);

			builder.Property(x => x.IssueDate).HasColumnType("date");
			builder.Property(x => x.ExpiryDate).HasColumnType("date");

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

			builder.HasOne<Supplier>()
				   .WithMany()
				   .HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict)
				   .IsRequired();
		}
	}

	// SupplierRisk
	public class SupplierRiskConfiguration : IEntityTypeConfiguration<SupplierRisk>
	{
		public void Configure(EntityTypeBuilder<SupplierRisk> builder)
		{
			builder.HasKey(x => x.RiskID);
			builder.Property(x => x.RiskID).ValueGeneratedOnAdd();

			builder.Property(x => x.RiskType).HasMaxLength(50).IsRequired();
			builder.Property(x => x.Score).HasPrecision(5, 2);
			builder.Property(x => x.AssessedDate).HasColumnType("date");
			builder.Property(x => x.Notes).HasMaxLength(500);

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

			builder.HasOne<Supplier>()
				   .WithMany()
				   .HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict)
				   .IsRequired();
		}
	}
}