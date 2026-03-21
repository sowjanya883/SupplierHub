namespace SupplierHub.DTOs.ShippingDto
{
	public class DeliverySlotReadDto
	{
		public long SlotID { get; set; }
		public long SiteID { get; set; }
		public DateTime Date { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
		public int? Capacity { get; set; }
		public string Status { get; set; } = null!;
		public DateTime UpdatedOn { get; set; }
	}
}