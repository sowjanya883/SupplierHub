using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierRiskDTO
{
	public class SupplierRiskGetAllDto
	{
		public long RiskID { get; set; }
		public required string RiskType { get; set; }
		public decimal? Score { get; set; }
		public required string Status { get; set; }
	}
}