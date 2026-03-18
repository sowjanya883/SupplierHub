using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.NotificationDTO
{
	public class NotificationUpdateDto
	{
		// Typically you do not allow changing UserID in an update; include only if needed.
		// public long UserID { get; set; }

		[Required, MaxLength(500)]
		public string Message { get; set; } = default!;

		[MaxLength(30)]
		public string? Category { get; set; }

		public long? RefEntityID { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = default!;
	}
}
