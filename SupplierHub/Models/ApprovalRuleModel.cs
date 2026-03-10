using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplierHub.Constants.Enum;
namespace SupplierHub.Models
{
	public class ApprovalRuleModel
	{
		[Key]
		public int? RuleID { get; set; }
		[Column(TypeName ="nvarchar(20)")]
		public string? Scope { get; set; }
		[Column(TypeName ="nvarchar(max)")]
		public string? ExpressionJSON { get; set; }
		[Column(TypeName ="nvarchar(20)")]
		public RuleSeverity Severity { get; set; }

		public bool? Status { get; set; } = false;
	}
}
