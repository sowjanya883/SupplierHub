using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Ncr
	{
		[Key]
		public long NcrID { get; set; }

		[Required]
		public long GrnItemID { get; set; }

		[MaxLength(100)]
		public string? DefectType { get; set; }

		[MaxLength(20)]
		public string? Severity { get; set; }

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