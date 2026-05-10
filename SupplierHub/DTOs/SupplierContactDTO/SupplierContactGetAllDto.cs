namespace SupplierHub.DTOs.SupplierContactDTO
{
	public class SupplierContactGetAllDto
	{
		public long ContactID { get; set; }

		public long SupplierID { get; set; }

		public required string SupplierName { get; set; }

		public string? Email { get; set; }

		public string? Phone { get; set; }

		public string? Role { get; set; }

		public required string Status { get; set; }
	}
}
