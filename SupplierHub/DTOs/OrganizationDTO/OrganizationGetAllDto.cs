using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.OrganizationDTO
{
	public class OrganizationGetAllDto
	{
		public long OrgID { get; set; }

		public required string OrganizationName { get; set; }
		public string? TaxID { get; set; }

		public required string Status { get; set; }
	}
}