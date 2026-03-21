using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ShippingDto
{
	public class ShipmentUpdateDto
	{
		public DateTime? ShipDate { get; set; }

		[MaxLength(100)]
		public string? Carrier { get; set; }

		[MaxLength(100)]
		public string? TrackingNo { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = null!;
	}
}