using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.OrganizationDTO
{
	public class OrganizationCreateDto
	{
		public required string OrganizationName { get; set; }
		public string? AddressJson { get; set; }
		public string? TaxID { get; set; }
		public required string Status { get; set; }
	}
}