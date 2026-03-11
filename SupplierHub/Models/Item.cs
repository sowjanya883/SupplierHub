using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("item")]
	[Index(nameof(CategoryId), Name = "idx_item_category")]
	[Index(nameof(Sku), IsUnique = true, Name = "uq_item_sku")]
	public class Item
	{
		[Key]
		public long ItemId { get; set; }

		[Required]
		public long CategoryId { get; set; }

		[Required, MaxLength(100)]
		public string Sku { get; set; } = default!;

		[MaxLength(500)]
		public string? Description { get; set; }

		[MaxLength(30)]
		public string? Uom { get; set; }

		public int? LeadTimeDays { get; set; }

		// JSON
		public string? SpecsJson { get; set; }

		[Required]
		public ItemStatus Status { get; set; } = ItemStatus.Active;

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedOn { get; set; }

		// Navigation
		[ForeignKey(nameof(CategoryId))]
		public Category Category { get; set; } = default!;

		public ICollection<CatalogItem> CatalogItems { get; set; } = new List<CatalogItem>();
		public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

		public ICollection<RFxLine> RFxLines { get; set; }=new List<RFxLine>();
	}
}
