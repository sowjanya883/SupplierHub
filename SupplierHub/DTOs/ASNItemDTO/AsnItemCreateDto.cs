using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ShippingDto
{
	public class AsnItemCreateDto
	{
		[Required]
		public long AsnID { get; set; }

		[Required]
		public long PoLineID { get; set; }

		public decimal? ShippedQty { get; set; }

		[MaxLength(100)]
		public string? LotBatch { get; set; }

		public string? SerialJson { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = "SHIPPED";
	}
}