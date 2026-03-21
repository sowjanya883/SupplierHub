using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierDTO
{
	public class SupplierCreateDto
	{

		public required string LegalName { get; set; }

		public string? DunsOrRegNo { get; set; }

		public string? TaxID { get; set; }

		public string? BankInfoJson { get; set; }

		public long? PrimaryContactID { get; set; }

		public required string Status { get; set; }
	}
}