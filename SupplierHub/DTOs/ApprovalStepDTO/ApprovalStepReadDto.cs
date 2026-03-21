namespace SupplierHub.DTOs.ApprovalDto
{
	public class ApprovalStepReadDto
	{
		public long StepID { get; set; }
		public long PrID { get; set; }
		public long ApproverID { get; set; }
		public string Decision { get; set; } = null!;
		public DateTime? DecisionDate { get; set; }
		public string? Remarks { get; set; }
		public string Status { get; set; } = null!;
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}