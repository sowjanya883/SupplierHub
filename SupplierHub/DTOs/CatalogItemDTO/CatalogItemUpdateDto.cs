using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.CatalogItemDTO
{
	public class CatalogItemUpdateDto : CatalogItemCreateDto
	{
		public long CatItemID { get; set; }
	}
}