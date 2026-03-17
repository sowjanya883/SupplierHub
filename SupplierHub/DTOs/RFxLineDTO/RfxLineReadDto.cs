namespace SupplierHub.DTOs.RFxLineDTO
{
	public class RfxLineReadDto
	{
		public long RfxLineID { get; set; }
		public long RfxID { get; set; }
		public long? ItemID { get; set; }

		
		public string? ItemName { get; set; }

		public decimal? Qty { get; set; }
		public string? Uom { get; set; }
		public decimal? TargetPrice { get; set; }
		public string? Status { get; set; }
	}
}
