using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("item")]
	public class Item
	{
		[Key]
		public long ItemId { get; set; }

		[Required]
		public long CategoryId { get; set; }

		[Required, MaxLength(100)]
		public string Sku { get; set; }

		[MaxLength(500)]
		public string? Description { get; set; }

		[MaxLength(30)]
		public string? Uom { get; set; }

		public int? LeadTimeDays { get; set; }

		public string? SpecsJson { get; set; }

		[Required]
		public ItemStatus Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public bool IsDeleted { get; set; }  // default -> false

		// Navigation
		[ForeignKey(nameof(CategoryId))]
		public virtual Category Category { get; set; }

		public virtual ICollection<CatalogItem> CatalogItems { get; set; }
		public virtual ICollection<Contract> Contracts { get; set; }
		public virtual ICollection<RFxLine> RFxLines { get; set; }
	}
}