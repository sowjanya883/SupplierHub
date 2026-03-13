using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class ComplianceDoc
	{
		[Key]
		public long DocID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		[Required, MaxLength(50)]
		public required string DocType { get; set; }

		[MaxLength(500)]
		public string? FileUri { get; set; }

		public DateTime? IssueDate { get; set; }

		public DateTime? ExpiryDate { get; set; }

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