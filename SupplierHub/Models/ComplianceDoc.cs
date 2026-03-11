using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("compliance_doc")]

	[Index(nameof(SupplierId), Name = "idx_comp_supplier")]
	[Index(nameof(DocType), Name = "idx_comp_doctype")]
	[Index(nameof(ExpiryDate), Name = "idx_comp_expiry")]
	public class ComplianceDoc
	{
		[Key]
		public long DocId { get; set; }

		[Required]
		public long SupplierId { get; set; } // FK

		[Required, MaxLength(50)]
		public string DocType { get; set; } = default!;

		[MaxLength(500)]
		public string? FileUri { get; set; }

		public DateOnly? IssueDate { get; set; }

		public DateOnly? ExpiryDate { get; set; }

		[Required]
		public ComplianceDocStatus Status { get; set; } = ComplianceDocStatus.Active;

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedOn { get; set; }

		// Navigation
		[ForeignKey(nameof(SupplierId))]
		public virtual Supplier Supplier { get; set; } = default!;
	}
}