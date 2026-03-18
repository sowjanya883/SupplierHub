using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.NotificationDTO
{
	public class NotificationCreateDto
	{
		[Required]
		public long UserID { get; set; }

		[Required, MaxLength(500)]
		public string Message { get; set; } = default!;

		[MaxLength(30)]
		public string? Category { get; set; }

		public long? RefEntityID { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = default!;

		// Optional: allow client to pass CreatedDate if you need a domain-specific date
		// If you always want server time, remove this and set in mapping/service.
		public DateTime? CreatedDate { get; set; }
	}
}