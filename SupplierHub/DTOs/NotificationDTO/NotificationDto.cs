using System;

namespace SupplierHub.DTOs.NotificationDTO
{
	public class NotificationDto
	{
		public long NotificationID { get; set; }
		public long UserID { get; set; }
		public string Message { get; set; } = default!;
		public string? Category { get; set; }
		public long? RefEntityID { get; set; }
		public string Status { get; set; } = default!;
		public DateTime CreatedDate { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
		public bool IsDeleted { get; set; }
	}
}
