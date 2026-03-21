using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ShippingDto
{
	public class AsnItemUpdateDto
	{
		public decimal? ShippedQty { get; set; }

		[MaxLength(100)]
		public string? LotBatch { get; set; }

		public string? SerialJson { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = null!;
	}
}