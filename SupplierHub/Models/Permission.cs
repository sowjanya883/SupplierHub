using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Permission
	{
		[Key]
		public long PermissionID { get; set; }

		[Required, MaxLength(120)]
		public string Code { get; set; }

		[Required, MaxLength(150)]
		public required string PermissionName { get; set; }

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