using System.ComponentModel.DataAnnotations;
namespace SupplierHub.DTOs.CatalogItemDTO
{
	public class CatalogItemGetAllDto	{
		public long CatItemID { get; set; }

		public long CatalogID { get; set; }

		public long ItemID { get; set; }
		public decimal Price { get; set; }

		public decimal? MinOrderQty { get; set; }

		public required string Currency { get; set; }

		public required string Status { get; set; }
	}
}
 