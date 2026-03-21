using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.PoLineDTO
{
	public class PoLineCreateDto
	{
		[Required]
		public long PoId { get; set; }

		public long? ItemId { get; set; }

		[StringLength(500)]
		public string? Description { get; set; }

		public decimal? Qty { get; set; }

		[StringLength(30)]
		public string? UoM { get; set; }

		public decimal? UnitPrice { get; set; }

		public decimal? LineTotal { get; set; }

		public DateTime? DeliveryDate { get; set; }

		[Required]
		[StringLength(30)]
		public string Status { get; set; } = string.Empty;
	}
}
