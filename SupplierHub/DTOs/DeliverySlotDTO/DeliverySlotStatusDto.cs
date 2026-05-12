using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ShippingDto
{
	/// <summary>
	/// Lightweight payload for changing only a delivery slot's status
	/// (AVAILABLE / BOOKED / CLOSED). Avoids requiring the full slot details
	/// the way DeliverySlotUpdateDto does.
	/// </summary>
	public class DeliverySlotStatusDto
	{
		[Required, MaxLength(30)]
		public string Status { get; set; } = null!;
	}
}
