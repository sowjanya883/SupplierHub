using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.OrganizationDTO
{
	public class OrganizationCreateDto
	{
		[Required, MaxLength(200)]
		public required string OrganizationName { get; init; }

		[MaxLength(50)]
		public string? TaxID { get; init; }

		public string? AddressJson { get; init; }
	}
}