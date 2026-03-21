using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierRiskDTO
{
	public class SupplierRiskUpdateDto : SupplierRiskCreateDto
	{
		public long RiskID { get; set; }
	}
}