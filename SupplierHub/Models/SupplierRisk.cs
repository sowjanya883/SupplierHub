using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("supplier_risk")]

	[Index(nameof(SupplierId), Name = "idx_risk_supplier")]
	[Index(nameof(RiskType), Name = "idx_risk_type")]
	[Index(nameof(AssessedDate), Name = "idx_risk_assessed")]
	public class SupplierRisk
	{
		[Key]
		public long RiskId { get; set; }

		[Required]
		public long SupplierId { get; set; } // FK

		[Required, MaxLength(50)]
		public string RiskType { get; set; } = default!;

		[Range(typeof(decimal), "0", "999.99")]
		public decimal? Score { get; set; } // decimal(5,2)

		public DateOnly? AssessedDate { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

		[Required]
		public RiskStatus Status { get; set; } = RiskStatus.Active;

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedOn { get; set; }

		// Navigation
		[ForeignKey(nameof(SupplierId))]
		public virtual Supplier Supplier { get; set; } = default!;
	}
}