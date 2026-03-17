using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class PrLine
	{
		[Key]
		public long PrLineID { get; set; }

		[Required]
		public long PrID { get; set; }

		public long? ItemID { get; set; }

		[MaxLength(500)]
		public string? Description { get; set; }

		public decimal? Qty { get; set; }

		[MaxLength(30)]
		public string? Uom { get; set; }

		public decimal? TargetPrice { get; set; }

		[MaxLength(10)]
		public string? Currency { get; set; }

		public long? SupplierPreferredID { get; set; }

<<<<<<< HEAD
		[StringLength(500)]
		public string Notes { get; set; }
		public bool IsDeleted { get; set; }  // default -> false
=======
		[MaxLength(500)]
		public string? Notes { get; set; }
>>>>>>> f5b24b19b20cc4f606a8ea7902667aadcbaffb0f

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