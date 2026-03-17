using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class ApprovalStep
	{
		[Key]
		public long StepID { get; set; }

		[Required]
		public long PrID { get; set; }

		[Required]
		public long ApproverID { get; set; }

		[Required, MaxLength(30)]
		public required string Decision { get; set; }

		public DateTime? DecisionDate { get; set; }

<<<<<<< HEAD
		[StringLength(500)]
		public string Remarks { get; set; }
		public bool IsDeleted { get; set; }  // default -> false
=======
		[MaxLength(500)]
		public string? Remarks { get; set; }
>>>>>>> f5b24b19b20cc4f606a8ea7902667aadcbaffb0f

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}