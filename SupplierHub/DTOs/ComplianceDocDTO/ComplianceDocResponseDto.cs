using System;

namespace SupplierHub.DTOs.ComplianceDocDTO
{
	public class ComplianceDocResponseDto : ComplianceDocCreateDto
	{
		public long DocID { get; init; }
		public string Status { get; init; } = string.Empty;
		public DateTime CreatedOn { get; init; }
		public DateTime UpdatedOn { get; init; }
		public bool IsDeleted { get; init; }
	}
}