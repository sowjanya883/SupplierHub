namespace SupplierHub.DTOs.ComplianceDocDTO
{
	public class ComplianceDocCreateDto
	{
		public long SupplierID { get; set; }
		public required string DocType { get; set; }
		public string? FileUri { get; set; }
		public DateTime? IssueDate { get; set; }
		public DateTime? ExpiryDate { get; set; }
		public required string Status { get; set; }
	}
}