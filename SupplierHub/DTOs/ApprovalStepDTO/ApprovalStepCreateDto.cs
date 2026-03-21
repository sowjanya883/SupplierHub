using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ApprovalDto
{
	public class ApprovalStepCreateDto
	{
		[Required]
		public long PrID { get; set; }

		[Required]
		public long ApproverID { get; set; }

		[Required, MaxLength(30)]
		public string Decision { get; set; } = "PENDING";

		[Required, MaxLength(30)]
		public string Status { get; set; } = "ACTIVE";

		[MaxLength(500)]
		public string? Remarks { get; set; }
	}
}