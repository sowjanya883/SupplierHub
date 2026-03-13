using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class RfxInvite
	{
		[Key]
		public long InviteID { get; set; }

		[Required]
		public long RfxID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		public DateTime? InvitedDate { get; set; }

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