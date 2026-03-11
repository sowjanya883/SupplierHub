using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("catalog")]
	[Index(nameof(Status), Name = "idx_catalog_status")]
	public class Catalog
	{
		[Key]
		public long CatalogId { get; set; }

		[Required]
		public long SupplierId { get; set; }

		[Required, MaxLength(200)]
		public string Name { get; set; } = default!;

		public DateOnly? ValidFrom { get; set; }

		public DateOnly? ValidTo { get; set; }

		[Required]
		public CatalogStatus Status { get; set; } = CatalogStatus.Active;

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedOn { get; set; }

		// Navigation
		[ForeignKey(nameof(SupplierId))]
		public Supplier Supplier { get; set; } = default!;

		public ICollection<CatalogItem> CatalogItems { get; set; } = new List<CatalogItem>();
	}
}