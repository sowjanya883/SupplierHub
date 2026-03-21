using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ContractDTO
{
	public class ContractCreateDto
	{
		public long SupplierID { get; set; }
		public long? ItemID { get; set; }

		public string? TermsJson { get; set; }
		public decimal? Rate { get; set; }

		public string? Currency { get; set; }

		public DateTime? ValidFrom { get; set; }

		public DateTime? ValidTo { get; set; }

		public required string Status { get; set; }
	}
}