using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.CatalogDTO
{
	public class CatalogGetAllDto
	{
		public long ItemID { get; set; }

		public required string Sku { get; set; }

		public required string Status { get; set; }
	}
}