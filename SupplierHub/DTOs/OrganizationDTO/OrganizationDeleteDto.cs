using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.OrganizationDTO
{
	public class OrganizationDeleteDto
	{
		public long? OrgID { get; set; }

		public string? OrganizationName { get; set; }

		public string? TaxID { get; set; }
	}
}