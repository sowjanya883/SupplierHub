namespace SupplierHub.DTOs.ComplianceDocDTO
{
	public class ComplianceDocGetAllDto
	{
		public long DocID { get; set; }
		public long SupplierID { get; set; }
		public required string DocType { get; set; }
		public required string Status { get; set; }
	}
}