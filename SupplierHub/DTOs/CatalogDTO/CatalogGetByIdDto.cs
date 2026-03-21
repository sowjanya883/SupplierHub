namespace SupplierHub.DTOs.CatalogDTO
{
	public class CatalogGetByIdDto : CatalogUpdateDto
	{
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}