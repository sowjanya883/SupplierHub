using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RfxEventDTO
{
	public class RFxEventCreateDto
	{
		[Required, MaxLength(10)]
		public string Type { get; set; } = string.Empty;

		[Required, MaxLength(200)]
		public string Title { get; set; } = string.Empty;

		public long? CategoryID { get; set; }

		public DateTime? OpenDate { get; set; }
		public DateTime? CloseDate { get; set; }
	}
}
