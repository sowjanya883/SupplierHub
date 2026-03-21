using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierRiskDTO
{
	public class SupplierRiskDeleteDto
	{
		public long? RiskID { get; set; }
		public long? SupplierID { get; set; }
		public string? RiskType { get; set; }
	}
}