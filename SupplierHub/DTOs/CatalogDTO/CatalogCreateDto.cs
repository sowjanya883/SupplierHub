using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.CatalogDTO
{
	public class CatalogCreateDto
	{
		public long CategoryID { get; set; }

		public required string Sku { get; set; }

		public string? Description { get; set; }

		public string? Uom { get; set; }
		public int? LeadTimeDays { get; set; }

		public string? SpecsJson { get; set; }

		public required string Status { get; set; }
	}
}