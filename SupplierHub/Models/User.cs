using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models.IAM
{
	[Table("Users")]
	public class User
	{
		[Key]
		public int UserId { get; set; }

		[Required, MaxLength(120)]
		public string Name { get; set; } = string.Empty;

		[Required, MaxLength(120)]
		public string Email { get; set; } = string.Empty;

		[MaxLength(30)]
		public string? Phone { get; set; }

		[Required, MaxLength(60)]
		public string Username { get; set; } = string.Empty;

		[Required]
		public UserRole Role { get; set; }

		[Required]
		public UserStatus Status { get; set; } = UserStatus.Active;

		// Security
		[Required]
		public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

		[Required]
		public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

		public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
		public DateTime? LastLoginAtUtc { get; set; }

		public ICollection<RFxEvent> Events { get; set; }
	}
}

