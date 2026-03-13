using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class SystemConfig
	{
		[Key]
		public long ConfigID { get; set; }

		[Required, MaxLength(100)]
		public required string ConfigKey { get; set; }

		[MaxLength(1000)]
		public string? Value { get; set; }

		[Required, MaxLength(20)]
		public required string Scope { get; set; }

		public long? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

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