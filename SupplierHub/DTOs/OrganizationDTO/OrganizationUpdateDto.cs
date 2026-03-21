using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.OrganizationDTO
{
	public class OrganizationUpdateDto : OrganizationCreateDto
	{
		public long OrgID { get; set; }
	}
}