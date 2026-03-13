using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierHub.Models;



// CatalogConfig.cs (Category, Item, Catalog, CatalogItem, Contract)

namespace SupplierHub.Config.Configurations
{
	// Category
	public class CategoryConfiguration : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.HasKey(x => x.CategoryID);
			builder.Property(x => x.CategoryID).ValueGeneratedOnAdd();

			builder.Property(x => x.CategoryName).HasMaxLength(200).IsRequired();

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Category>()
				   .WithMany()
				   .HasForeignKey(x => x.ParentCategoryID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}

	// Item
	public class ItemConfiguration : IEntityTypeConfiguration<Item>
	{
		public void Configure(EntityTypeBuilder<Item> builder)
		{
			builder.HasKey(x => x.ItemID);
			builder.Property(x => x.ItemID).ValueGeneratedOnAdd();

			builder.Property(x => x.Sku).HasMaxLength(100).IsRequired();
			builder.HasIndex(x => x.Sku).IsUnique();

			builder.Property(x => x.Description).HasMaxLength(500);
			builder.Property(x => x.Uom).HasMaxLength(30);
			builder.Property(x => x.LeadTimeDays);
			builder.Property(x => x.SpecsJson);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Category>()
				   .WithMany()
				   .HasForeignKey(x => x.CategoryID)
				   .OnDelete(DeleteBehavior.Restrict)
				   .IsRequired();
		}
	}

	// Catalog
	public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
	{
		public void Configure(EntityTypeBuilder<Catalog> builder)
		{
			builder.HasKey(x => x.CatalogID);
			builder.Property(x => x.CatalogID).ValueGeneratedOnAdd();

			builder.Property(x => x.CatalogName).HasMaxLength(200).IsRequired();

			builder.Property(x => x.ValidFrom).HasColumnType("date");
			builder.Property(x => x.ValidTo).HasColumnType("date");

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Supplier>()
				   .WithMany()
				   .HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict)
				   .IsRequired();
		}
	}

	// CatalogItem
	public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
	{
		public void Configure(EntityTypeBuilder<CatalogItem> builder)
		{
			builder.HasKey(x => x.CatItemID);
			builder.Property(x => x.CatItemID).ValueGeneratedOnAdd();

			builder.Property(x => x.Price).HasPrecision(18, 2).IsRequired();
			builder.Property(x => x.Currency).HasMaxLength(10).IsRequired();
			builder.Property(x => x.MinOrderQty).HasPrecision(18, 3);

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Catalog>()
				   .WithMany()
				   .HasForeignKey(x => x.CatalogID)
				   .OnDelete(DeleteBehavior.Restrict)
				   .IsRequired();

			builder.HasOne<Item>()
				   .WithMany()
				   .HasForeignKey(x => x.ItemID)
				   .OnDelete(DeleteBehavior.Restrict)
				   .IsRequired();

			// Excel lists both CatalogID and ItemID as UNIQUE; logical constraint is composite uniqueness.
			builder.HasIndex(x => new { x.CatalogID, x.ItemID }).IsUnique();
		}
	}

	// Contract
	public class ContractConfiguration : IEntityTypeConfiguration<Contract>
	{
		public void Configure(EntityTypeBuilder<Contract> builder)
		{
			builder.HasKey(x => x.ContractID);
			builder.Property(x => x.ContractID).ValueGeneratedOnAdd();

			builder.Property(x => x.TermsJson);
			builder.Property(x => x.Rate).HasPrecision(18, 4);
			builder.Property(x => x.Currency).HasMaxLength(10);

			builder.Property(x => x.ValidFrom).HasColumnType("date");
			builder.Property(x => x.ValidTo).HasColumnType("date");

			builder.Property(x => x.Status).HasMaxLength(30).IsRequired()
				   .HasDefaultValue("ACTIVE");

			builder.Property(x => x.CreatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .IsRequired();

			builder.Property(x => x.UpdatedOn)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP")
				   .ValueGeneratedOnAddOrUpdate()
				   .IsRequired();

			builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();

			builder.HasOne<Supplier>()
				   .WithMany()
				   .HasForeignKey(x => x.SupplierID)
				   .OnDelete(DeleteBehavior.Restrict)
				   .IsRequired();

			builder.HasOne<Item>()
				   .WithMany()
				   .HasForeignKey(x => x.ItemID)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}