namespace SupplierHub.DTOs.CatalogDTO
{
	public class CatalogCreateDto
	{
		public long SupplierID { get; set; }
		public required string CatalogName { get; set; }
		public DateTime? ValidFrom { get; set; }
		public DateTime? ValidTo { get; set; }
		public required string Status { get; set; }
	}
}