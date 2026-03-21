using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ItemDTO
{
	public class ItemUpdateDto : ItemCreateDto
	{
		public long ItemID { get; set; }
	}
}