using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SupplierHub.DTOs.UserDTO
{
	/// <summary>
	/// Request model for updating an existing user.
	/// All fields optional to support PATCH-like behavior.
	/// </summary>
	public class UpdateUserDto
	{
		public long? OrgID { get; set; }

		[MaxLength(150)]
		public string? UserName { get; set; }

		[MaxLength(150), EmailAddress]
		public string? Email { get; set; }

		[MaxLength(30), Phone]
		public string? Phone { get; set; }

		/// <summary>
		/// If present, hash to PasswordHash before persisting.
		/// </summary>
		[MinLength(8), MaxLength(255)]
		public string? Password { get; set; }

		[MaxLength(30)]
		public string? Status { get; set; }

		/// <summary>
		/// Soft delete toggle (optional).
		/// </summary>
		public bool? IsDeleted { get; set; }
        
        /// <summary>
        /// Optional list of role names assigned to the user.
        /// When provided, service will replace the current user-role list.
        /// </summary>
       
	}
}
