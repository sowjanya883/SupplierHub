using System;

namespace SupplierHub.DTOs.SystemConfigDTO
{
	public class SystemConfigReadDto
	{
		public long ConfigID { get; set; }
		public string ConfigKey { get; set; } = string.Empty;
		public string? Value { get; set; }
		public string Scope { get; set; } = string.Empty;
		public long? UpdatedBy { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public string Status { get; set; } = string.Empty;
		public bool IsDeleted { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}
