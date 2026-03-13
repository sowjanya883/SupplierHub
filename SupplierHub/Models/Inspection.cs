using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Inspection
	{
		[Key]
		public long InspID { get; set; }

		[Required]
		public long GrnItemID { get; set; }

		[MaxLength(10)]
		public string? Result { get; set; }

		public long? InspectorID { get; set; }

		public DateTime? InspDate { get; set; }

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