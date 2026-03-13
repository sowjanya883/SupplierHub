using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class CatalogItem
	{
		[Key]
		public long CatItemID { get; set; }

		[Required]
		public long CatalogID { get; set; }

		[Required]
		public long ItemID { get; set; }

		[Required]
		public decimal Price { get; set; }

		[Required, MaxLength(10)]
		public required string Currency { get; set; }

		public decimal? MinOrderQty { get; set; }

		public string? PriceBreaksJson { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}