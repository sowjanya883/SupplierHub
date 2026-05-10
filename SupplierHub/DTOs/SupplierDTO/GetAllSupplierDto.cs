using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierDTO
{
	public class GetAllSupplierDto
	{
		public long SupplierID { get; set; }

		public required string LegalName { get; set; }
		public string? TaxID { get; set; }
		public required string Status { get; set; }
	}
}