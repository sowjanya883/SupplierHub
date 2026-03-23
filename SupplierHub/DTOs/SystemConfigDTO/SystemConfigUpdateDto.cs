using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SystemConfigDTO
{
	public class SystemConfigUpdateDto
	{
		[Required]
		public long ConfigID { get; set; }

		[MaxLength(1000)]
		public string? Value { get; set; }

		[MaxLength(30)]
		public string? Status { get; set; }

		public long? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
