using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ShippingDto
{
	public class ShipmentCreateDto
	{
		[Required]
		public long PoID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		public DateTime? ShipDate { get; set; }

		[MaxLength(100)]
		public string? Carrier { get; set; }

		[MaxLength(100)]
		public string? TrackingNo { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = "PENDING";
	}
}