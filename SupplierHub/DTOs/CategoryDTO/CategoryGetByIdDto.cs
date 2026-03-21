namespace SupplierHub.DTOs.CategoryDTO
{
	public class CategoryGetByIdDto : CategoryUpdateDto
	{
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}