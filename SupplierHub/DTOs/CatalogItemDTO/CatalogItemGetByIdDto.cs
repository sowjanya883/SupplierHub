namespace SupplierHub.DTOs.CatalogItemDTO
{
	public class CatalogItemGetByIdDto : CatalogItemUpdateDto
	{
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}