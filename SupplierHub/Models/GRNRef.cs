using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class GrnRef
	{
		[Key]
		public long GrnID { get; set; }

		[Required]
		public long PoID { get; set; }

		public long? AsnID { get; set; }

		public DateTime? ReceivedDate { get; set; }

		public long? ReceivedBy { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}