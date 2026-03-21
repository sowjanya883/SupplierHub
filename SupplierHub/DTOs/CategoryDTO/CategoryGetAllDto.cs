namespace SupplierHub.DTOs.CategoryDTO
{
	public class CategoryGetAllDto
	{
		public long CategoryID { get; set; }
		public required string CategoryName { get; set; }
		public required string Status { get; set; }
	}
}