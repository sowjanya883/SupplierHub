using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class PoLine
	{
		[Key]
		public long PoLineID { get; set; }

		[Required]
		public long PoID { get; set; }

		public long? ItemID { get; set; }

		[MaxLength(500)]
		public string? Description { get; set; }

		public decimal? Qty { get; set; }

		[MaxLength(30)]
		public string? Uom { get; set; }

		public decimal? UnitPrice { get; set; }

		public decimal? LineTotal { get; set; }

		public DateTime? DeliveryDate { get; set; }

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