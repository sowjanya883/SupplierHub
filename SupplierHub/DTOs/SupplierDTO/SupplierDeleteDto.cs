using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierDTO
{
	public class SupplierDeleteDto
	{
		public long? SupplierID { get; set; }

		public string? LegalName { get; set; }

		public string? TaxID { get; set; }

		public string? DunsOrRegNo { get; set; }
	}
}