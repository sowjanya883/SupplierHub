namespace SupplierHub.DTOs.ShippingDto
{
	public class AsnItemReadDto
	{
		public long AsnItemID { get; set; }
		public long AsnID { get; set; }
		public long PoLineID { get; set; }
		public decimal? ShippedQty { get; set; }
		public string? LotBatch { get; set; }
		public string? SerialJson { get; set; }
		public string? Notes { get; set; }
		public string Status { get; set; } = null!;
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}