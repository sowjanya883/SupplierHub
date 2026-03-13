using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class BidLine
	{
		[Key]
		public long BidLineID { get; set; }

		[Required]
		public long BidID { get; set; }

		[Required]
		public long RfxLineID { get; set; }

		public decimal? UnitPrice { get; set; }

		public int? LeadTimeDays { get; set; }

		[MaxLength(10)]
		public string? Currency { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}