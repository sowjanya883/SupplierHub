using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ApprovalDto
{
	public class ApprovalStepUpdateDto
	{
		[Required, MaxLength(30)]
		public string Decision { get; set; } = null!;

		public DateTime? DecisionDate { get; set; }

		[MaxLength(500)]
		public string? Remarks { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = null!;
	}
}