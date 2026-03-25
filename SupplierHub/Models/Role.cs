using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Role
	{
		[Key]
		public long RoleID { get; set; }

		[Required, MaxLength(100)]
		public required string RoleName { get; set; }

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