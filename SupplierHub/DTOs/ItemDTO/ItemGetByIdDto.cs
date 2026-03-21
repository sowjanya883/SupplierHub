namespace SupplierHub.DTOs.ItemDTO
{
	public class ItemGetByIdDto : ItemUpdateDto
	{
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}