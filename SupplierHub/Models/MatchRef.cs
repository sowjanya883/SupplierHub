using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class MatchRef
	{
		[Key]
		public long MatchID { get; set; }

		[Required]
		public long InvoiceID { get; set; }

		public long? PoID { get; set; }

		public long? GrnID { get; set; }

		[MaxLength(20)]
		public string? Result { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}