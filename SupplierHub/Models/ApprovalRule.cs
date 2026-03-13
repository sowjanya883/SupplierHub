using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class ApprovalRule
	{
		[Key]
		public long RuleID { get; set; }

		[Required]
		[MaxLength(30)]
		public required string Scope { get; set; }

		[Required]
		[MaxLength(10)]
		public required string Severity { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}