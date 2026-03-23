using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ApprovalRuleDTO
{
	public class ApprovalRuleUpdateDto
	{
		[Required]
		public long RuleID { get; set; }

		[MaxLength(30)]
		public string? Scope { get; set; }

		[MaxLength(10)]
		public string? Severity { get; set; }

		public bool? IsDeleted { get; set; }
	}
}
