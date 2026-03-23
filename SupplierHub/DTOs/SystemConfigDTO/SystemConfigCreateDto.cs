using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SystemConfigDTO
{
	public class SystemConfigCreateDto
	{
		[Required, MaxLength(100)]
		public string ConfigKey { get; set; } = string.Empty;

		[MaxLength(1000)]
		public string? Value { get; set; }

		[Required, MaxLength(20)]
		public string Scope { get; set; } = string.Empty;

		[Required, MaxLength(30)]
		public string Status { get; set; } = string.Empty;
	}
}
