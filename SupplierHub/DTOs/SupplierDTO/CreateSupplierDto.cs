using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierDTO
{ 

	// Create (POST) payload
	public class CreateSupplierDto
	{
		[Required, MaxLength(200)]
		public required string LegalName { get; init; }

		[MaxLength(50)]
		public string? DunsOrRegNo { get; init; }

		[MaxLength(50)]
		public string? TaxID { get; init; }

		public string? BankInfoJson { get; init; }

		public long? PrimaryContactID { get; init; }
	}
}