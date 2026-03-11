using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplierHub.Models.IAM;

namespace SupplierHub.Models
{
	[Table("ApprovalStep")]
	public class ApprovalStep
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long StepID { get; set; }

		[Required]
		[ForeignKey(nameof(Requisition))]
		public long PRID { get; set; }

		[Required]
		[ForeignKey(nameof(Approver))]
		public long ApproverID { get; set; }

		[Required]
		[StringLength(20)]
		public string Decision { get; set; }

		public DateTime? DecisionDate { get; set; }

		[StringLength(500)]
		public string Remarks { get; set; }

		// Navigation Properties
		public virtual Requisition Requisition { get; set; }

		public virtual User Approver { get; set; }
	}
}