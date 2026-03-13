using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class ErpExportRef
	{
		[Key]
		public long ErprefID { get; set; }

		[Required, MaxLength(30)]
		public required string EntityType { get; set; }

		[MaxLength(500)]
		public string? PayloadUri { get; set; }

		[MaxLength(100)]
		public string? CorrelationID { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		public DateTime? ExportDate { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}