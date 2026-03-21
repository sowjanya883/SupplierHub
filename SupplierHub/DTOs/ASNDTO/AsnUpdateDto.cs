using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ShippingDto
{
	public class AsnUpdateDto
	{
		[MaxLength(100)]
		public string? AsnNo { get; set; }

		public DateTime? CreatedDate { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = null!;
	}
}