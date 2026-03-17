namespace SupplierHub.DTOs.RequisitionDto
{
	public class RequisitionReadDto
	{
		public long PrID { get; set; }
		public long RequesterID { get; set; }
		public long OrgID { get; set; }
		public string? CostCenter { get; set; }
		public string? Justification { get; set; }
		public DateTime? RequestedDate { get; set; }
		public DateTime? NeededByDate { get; set; }
		public string Status { get; set; } = null!;
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}