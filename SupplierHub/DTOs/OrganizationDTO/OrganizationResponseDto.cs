using System;

namespace SupplierHub.DTOs.OrganizationDTO
{
	public class OrganizationResponseDto : OrganizationCreateDto
	{
		public long OrgID { get; init; }
		public string? Status { get; init; }
		public DateTime CreatedOn { get; init; }
		public DateTime UpdatedOn { get; init; }
		public bool IsDeleted { get; init; }
	}
}