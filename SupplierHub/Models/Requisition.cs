using System;
using System.ComponentModel.DataAnnotations;
using SupplierHub.Constants; // Import your enum namespace
//....
namespace SupplierHub.Models
{
	public class Requisition
	{
		[Key]
		public long PRID { get; set; }

		[Required]
		public long RequesterID { get; set; }

		[Required]
		public long OrgID { get; set; }

		[StringLength(100)]
		public string CostCenter { get; set; }

		public string Justification { get; set; }

		[Required]
		public DateTimeOffset RequestedDate { get; set; } = DateTimeOffset.Now;

		public DateTimeOffset? NeededByDate { get; set; }

		[Required]
		public RequisitionStatus Status { get; set; } = RequisitionStatus.Draft;
	}
}