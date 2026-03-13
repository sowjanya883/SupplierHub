using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class RfxEvent
	{
		[Key]
		public long RfxID { get; set; }

		[Required, MaxLength(10)]
		public required string Type { get; set; }

		[Required, MaxLength(200)]
		public required string Title { get; set; }

		public long? CategoryID { get; set; }

		public long? CreatedBy { get; set; }

		public DateTime? OpenDate { get; set; }

		public DateTime? CloseDate { get; set; }

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