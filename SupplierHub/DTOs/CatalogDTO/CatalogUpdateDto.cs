using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.CatalogDTO
{
	public class CatalogUpdateDto : CatalogCreateDto
	{
		public long ItemID { get; set; }
	}
}