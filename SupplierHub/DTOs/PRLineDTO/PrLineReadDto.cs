namespace SupplierHub.DTOs.RequisitionDto
{
	public class PrLineReadDto
	{
		public long PrLineID { get; set; }
		public long PrID { get; set; }
		public long? ItemID { get; set; }
		public string? Description { get; set; }
		public decimal? Qty { get; set; }
		public string? Uom { get; set; }
		public decimal? TargetPrice { get; set; }
		public string? Currency { get; set; }
		public long? SupplierPreferredID { get; set; }
		public string? Notes { get; set; }
		public string Status { get; set; } = null!;
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}