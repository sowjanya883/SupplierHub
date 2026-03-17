using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.OrganizationDTO
{
	public class OrganizationUpdateDto : OrganizationCreateDto
	{
		[Required]
		public long OrgID { get; init; }
	}
}