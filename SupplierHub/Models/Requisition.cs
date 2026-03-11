using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplierHub.Constants;
using SupplierHub.Models.IAM;

namespace SupplierHub.Models
{
	[Table("Requisition")]
	public class Requisition
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long PRID { get; set; }

		[Required]
		[ForeignKey(nameof(Requester))]
		public long RequesterID { get; set; }

		[Required]
		[ForeignKey(nameof(Organization))]
		public long OrgID { get; set; }

		[Required]
		[StringLength(100)]
		public string CostCenter { get; set; }

		[StringLength(int.MaxValue)]
		public string Justification { get; set; }

		[Required]
		public DateTime RequestedDate { get; set; }

		public DateTime? NeededByDate { get; set; }

		[Required]
		[StringLength(20)]
		public string Status { get; set; }

		// Navigation Properties
		public virtual User Requester { get; set; }

		public virtual Organization Organization { get; set; }

		public virtual ICollection<PRLine> PRLines { get; set; } = new List<PRLine>();

		public virtual ICollection<ApprovalStep> ApprovalSteps { get; set; } = new List<ApprovalStep>();

		public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
	}
}