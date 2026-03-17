using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierRiskDTO
{
	public class SupplierRiskCreateDto
	{
		[Required]
		public long SupplierID { get; init; }

		[Required, MaxLength(50)]
		public required string RiskType { get; init; }

		public decimal? Score { get; init; }
		public DateTime? AssessedDate { get; init; }

		[MaxLength(500)]
		public string? Notes { get; init; }
	}
}