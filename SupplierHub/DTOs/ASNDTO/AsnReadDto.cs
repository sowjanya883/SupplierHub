namespace SupplierHub.DTOs.ShippingDto
{
	public class AsnReadDto
	{
		public long AsnID { get; set; }
		public long ShipmentID { get; set; }
		public string? AsnNo { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string Status { get; set; } = null!;
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}