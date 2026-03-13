using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class SupplierRisk
	{
		[Key]
		public long RiskID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		[Required, MaxLength(50)]
		public required string RiskType { get; set; }

		public decimal? Score { get; set; }

		public DateTime? AssessedDate { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}