using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Award
	{
		[Key]
		public long AwardID { get; set; }

		[Required]
		public long RfxID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		public DateTime? AwardDate { get; set; }

		public decimal? AwardValue { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[MaxLength(10)]
		public string? Currency { get; set; }

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