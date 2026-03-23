using System;

namespace SupplierHub.DTOs.ApprovalRuleDTO
{
	public class ApprovalRuleReadDto
	{
		public long RuleID { get; set; }
		public string Scope { get; set; } = string.Empty;
		public string Severity { get; set; } = string.Empty;
		public bool IsDeleted { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}
