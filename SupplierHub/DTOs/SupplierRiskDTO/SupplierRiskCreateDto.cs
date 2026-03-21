using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierRiskDTO
{
	public class SupplierRiskCreateDto
	{
		public long SupplierID { get; set; }
		public required string RiskType { get; set; }
		public decimal? Score { get; set; }
		public DateTime? AssessedDate { get; set; }
		public string? Notes { get; set; }
		public required string Status { get; set; }
	}
}