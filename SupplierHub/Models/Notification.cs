using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Notification
	{
		[Key]
		public long NotificationID { get; set; }

		[Required]
		public long UserID { get; set; }

		[Required, MaxLength(500)]
		public required string Message { get; set; }

		[MaxLength(30)]
		public string? Category { get; set; }

		public long? RefEntityID { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedDate { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}