using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ApprovalRuleDTO
{
	public class ApprovalRuleCreateDto
	{
		[Required, MaxLength(30)]
		public string Scope { get; set; } = string.Empty;

		[Required, MaxLength(10)]
		public string Severity { get; set; } = string.Empty;
	}
}
