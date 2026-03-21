using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ItemDTO
{
	public class ItemGetAllDto
	{
		[Required]
		public long ItemID { get; set; }

		[Required]
		public required string Sku { get; set; }

		[Required]
		public required string Status { get; set; }
	}
}