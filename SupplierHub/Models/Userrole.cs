using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class UserRole
	{
		[Required]
		public long UserID { get; set; }

		[Required]
		public long RoleID { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}