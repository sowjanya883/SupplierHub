using System;
using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;

namespace SupplierHub.Models
{
	public class User
	{
		[Key]
		public long UserID { get; set; }

		public long? OrgID { get; set; }

		[Required, MaxLength(150)]
		public required string UserName { get; set; }

		[Required, MaxLength(150)]
		public required string Email { get; set; }

		[MaxLength(30)]
		public string? Phone { get; set; }

		[MaxLength(255)]
		public string? PasswordHash { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		//public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
	}
}