namespace SupplierHub.DTOs.ShippingDto
{
	public class ShipmentReadDto
	{
		public long ShipmentID { get; set; }
		public long PoID { get; set; }
		public long SupplierID { get; set; }
		public DateTime? ShipDate { get; set; }
		public string? Carrier { get; set; }
		public string? TrackingNo { get; set; }
		public string Status { get; set; } = null!;
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}