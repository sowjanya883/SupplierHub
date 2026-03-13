using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Item
	{
		[Key]
		public long ItemID { get; set; }

		[Required]
		public long CategoryID { get; set; }

		[Required, MaxLength(100)]
		public required string Sku { get; set; }

		[MaxLength(500)]
		public string? Description { get; set; }

		[MaxLength(30)]
		public string? Uom { get; set; }

		public int? LeadTimeDays { get; set; }

		public string? SpecsJson { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}