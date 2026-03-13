using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class AuditLog
	{
		[Key]
		public long AuditID { get; set; }

		public long? UserID { get; set; }

		[Required, MaxLength(100)]
		public required string Action { get; set; }

		[Required, MaxLength(200)]
		public required string Resource { get; set; }

		public string? Metadata { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

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