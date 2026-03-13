using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class PoAck
	{
		[Key]
		public long PocfmID { get; set; }

		[Required]
		public long PoID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		public DateTime? AcknowledgeDate { get; set; }

		[MaxLength(30)]
		public string? Decision { get; set; }

		[MaxLength(500)]
		public string? Counternotes { get; set; }

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