namespace SupplierHub.DTOs.SupplierRiskDTO
{
	public class SupplierRiskGetAllDto
	{
		public long RiskID { get; set; }

		public long SupplierID { get; set; }

		public required string RiskType { get; set; }

		public decimal? Score { get; set; }

		public DateTime? AssessedDate { get; set; }

		public string? Notes { get; set; }

		public required string Status { get; set; }
	}
}
