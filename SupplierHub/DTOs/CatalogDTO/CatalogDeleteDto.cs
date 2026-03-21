using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.CatalogDTO
{
	public class CatalogDeleteDto
	{
		public long? ItemID { get; set; }

		public long? CategoryID { get; set; }

		public string? Sku { get; set; }
	}
}