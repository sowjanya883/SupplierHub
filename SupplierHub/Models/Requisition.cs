using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	public class Requisition
	{
		[Key]
		public long PrID { get; set; }

		[Required]
		public long RequesterID { get; set; }

		[Required]
		public long RequesterUserID { get; set; } 

		[Required]
		public long OrgID { get; set; }

		[MaxLength(50)]
		public string? CostCenter { get; set; }

		[MaxLength(500)]
		public string? Justification { get; set; }

		public DateTime? RequestedDate { get; set; }

		public DateTime? NeededByDate { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		// Navigation Property - ONLY ONE DEFINITION ALLOWED
		[ForeignKey("RequesterUserID")]
		public virtual User Requester { get; set; } = null!;
	}
}