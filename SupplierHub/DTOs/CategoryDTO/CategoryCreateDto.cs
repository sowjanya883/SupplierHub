namespace SupplierHub.DTOs.CategoryDTO
{
	public class CategoryCreateDto
	{
		public long? ParentCategoryID { get; set; }
		public required string CategoryName { get; set; }
		public required string Status { get; set; }
	}
}