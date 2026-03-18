namespace SupplierHub.DTOs.RFxLineDTO
{
	public class RfxLineUpdateDto
	{
		
		public long RfxLineID { get; set; }
		public decimal? Qty { get; set; }
		public string? Uom { get; set; }
		public decimal? TargetPrice { get; set; }
		public string? Notes { get; set; }
		public string? Status { get; set; }
	}
}
