using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.CatalogItemDTO
{
	public class CatalogItemDeleteDto
	{
		public long? CatItemID { get; set; }

		public long? CatalogID { get; set; }

		public long? ItemID { get; set; }
	}
}