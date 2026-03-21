namespace SupplierHub.DTOs.ComplianceDocDTO
{
	public class ComplianceDocDeleteDto
	{
		public long? DocID { get; set; }
		public long? SupplierID { get; set; }
		public string? DocType { get; set; }
	}
}