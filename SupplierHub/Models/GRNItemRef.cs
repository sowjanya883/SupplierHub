using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class GrnItemRef
	{
		[Key]
		public long GrnItemID { get; set; }

		[Required]
		public long GrnID { get; set; }

		[Required]
		public long PoLineID { get; set; }

		public decimal? ReceivedQty { get; set; }

		public decimal? AcceptedQty { get; set; }

		public decimal? RejectedQty { get; set; }

		[MaxLength(200)]
		public string? Reason { get; set; }

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