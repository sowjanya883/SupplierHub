using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("catalog_item")]
	[Index(nameof(CatalogId), nameof(ItemId), IsUnique = true, Name = "uq_catalog_item_pair")]
	public class CatalogItem
	{
		[Key]
		public long CatItemId { get; set; }

		[Required]
		public long CatalogId { get; set; }

		[Required]
		public long ItemId { get; set; }

		[Required]
		public decimal Price { get; set; } // decimal(18,2)

		[Required, MaxLength(10)]
		public string Currency { get; set; } = default!;

		public decimal? MinOrderQty { get; set; }

		// JSON
		public string? PriceBreaksJson { get; set; }

		[Required]
		public CatalogItemStatus Status { get; set; } = CatalogItemStatus.Active;

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedOn { get; set; }

		// Navigation
		[ForeignKey(nameof(CatalogId))]
		public Catalog Catalog { get; set; } = default!;

		[ForeignKey(nameof(ItemId))]
		public Item Item { get; set; } = default!;
	}
}