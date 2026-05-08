using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.OrganizationDTO
{
	public class OrganizationGetByIdDto : OrganizationUpdateDto
	{
		public DateTime CreatedOn { get; set; }
		
		public DateTime UpdatedOn { get; set; }
	}
}