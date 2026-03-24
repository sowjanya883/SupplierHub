namespace SupplierHub.DTOs.CatalogDTO
{
	public class CatalogGetAllDto
	{
		public long CatalogID { get; set; }
		public required string CatalogName { get; set; }
		public required string Status { get; set; }
	}
}