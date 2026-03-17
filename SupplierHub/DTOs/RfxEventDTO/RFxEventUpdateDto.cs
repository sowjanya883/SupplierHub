using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SupplierHub.DTOs.RfxEventDTO
{
	public class RFxEventUpdateDto
	{
		[Required, MaxLength(200)]
		public string Title { get; set; } = string.Empty;

		public DateTime? CloseDate { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}
