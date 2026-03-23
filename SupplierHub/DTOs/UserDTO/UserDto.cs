using System;

namespace SupplierHub.DTOs.UserDTO
{
	/// <summary>
	/// Read model for user details (safe for responses).
	/// </summary>
	public class UserDto
	{
		public long UserID { get; set; }
		public long? OrgID { get; set; }

		public string UserName { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string? Phone { get; set; }

		/// <summary>
		/// Current status (e.g., Active, Inactive, Locked, etc.)
		/// </summary>
		public string Status { get; set; } = default!;

		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
		public bool IsDeleted { get; set; }

	}
}
