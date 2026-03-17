using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RFxLineDTO
{
	public class RfxLineCreateDto
	{
		[Required]
		public long RfxID { get; set; } 

		public long? ItemID { get; set; }

		[Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
		public decimal? Qty { get; set; }

		[MaxLength(30)]
		public string? Uom { get; set; }

		public decimal? TargetPrice { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }
	}
}
