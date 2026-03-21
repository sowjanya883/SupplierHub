using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ShippingDto
{
	public class AsnCreateDto
	{
		[Required]
		public long ShipmentID { get; set; }

		[MaxLength(100)]
		public string? AsnNo { get; set; }

		public DateTime? CreatedDate { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = "NEW";
	}
}