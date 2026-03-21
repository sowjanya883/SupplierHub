using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ShippingDto
{
	public class DeliverySlotCreateDto
	{
		[Required]
		public long SiteID { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required]
		public TimeSpan StartTime { get; set; }

		[Required]
		public TimeSpan EndTime { get; set; }

		public int? Capacity { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = "AVAILABLE";
	}
}