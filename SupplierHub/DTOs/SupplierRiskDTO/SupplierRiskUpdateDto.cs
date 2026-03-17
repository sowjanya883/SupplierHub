using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierRiskDTO
{
	public class SupplierRiskUpdateDto : SupplierRiskCreateDto
	{
		[Required]
		public long RiskID { get; init; }
	}
}