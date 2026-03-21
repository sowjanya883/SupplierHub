using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RequisitionDto
{
	public class PrLineCreateDto
	{
		[Required]
		public long PrID { get; set; }

		public long? ItemID { get; set; }

		[MaxLength(500)]
		public string? Description { get; set; }

		public decimal? Qty { get; set; }

		[MaxLength(30)]
		public string? Uom { get; set; }

		public decimal? TargetPrice { get; set; }

		[MaxLength(10)]
		public string? Currency { get; set; }

		public long? SupplierPreferredID { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = "PENDING";
	}
}