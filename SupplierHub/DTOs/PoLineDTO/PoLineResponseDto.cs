using System;

namespace SupplierHub.DTOs.PoLineDTO
{
	public class PoLineResponseDto
	{
		public long PoLineId { get; set; }
		public long PoId { get; set; }
		public long? ItemId { get; set; }
		public string? Description { get; set; }
		public decimal? Qty { get; set; }
		public string? UoM { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? LineTotal { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
		public bool IsDeleted { get; set; }
	}
}
