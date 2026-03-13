using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Bid
	{
		[Key]
		public long BidID { get; set; }

		[Required]
		public long RfxID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		public DateTime? BidDate { get; set; }

		public decimal? TotalValue { get; set; }

		[MaxLength(10)]
		public string? Currency { get; set; }

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