using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SupplierHub.DTOs.UserDTO
{
	/// <summary>
	/// Request model for creating a new user.
	/// </summary>
	public class CreateUserDto
	{
		public long? OrgID { get; set; }

		[Required, MaxLength(150)]
		public string UserName { get; set; } = default!;

		[Required, MaxLength(150), EmailAddress]
		public string Email { get; set; } = default!;

		[MaxLength(30), Phone]
		public string? Phone { get; set; }

		/// <summary>
		/// Plain text password; hash this server-side and store into PasswordHash.
		/// </summary>
		[Required, MinLength(8), MaxLength(255)]
		public string Password { get; set; } = default!;

		/// <summary>
		/// E.g., "Active" or "Inactive". Consider using an enum for stricter control.
		/// </summary>
		[Required, MaxLength(30)]
		public string Status { get; set; } = "Active";

	}
}