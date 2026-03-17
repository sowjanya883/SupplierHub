using System;

namespace SupplierHub.DTOs.SupplierRiskDTO
{
	public class SupplierRiskResponseDto : SupplierRiskCreateDto
	{
		public long RiskID { get; init; }
		public string Status { get; init; } = string.Empty;
		public DateTime CreatedOn { get; init; }
		public DateTime UpdatedOn { get; init; }
		public bool IsDeleted { get; init; }
	}
}