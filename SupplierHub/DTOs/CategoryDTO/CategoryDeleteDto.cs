namespace SupplierHub.DTOs.CategoryDTO
{
	public class CategoryDeleteDto
	{
		public long? CategoryID { get; set; }
		public long? ParentCategoryID { get; set; }
		public string? CategoryName { get; set; }
	}
}