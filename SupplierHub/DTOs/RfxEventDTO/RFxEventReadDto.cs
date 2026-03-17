namespace SupplierHub.DTOs.RfxEventDTO
{
	public class RFxEventReadDto
	{
		public long RfxID { get; set; }
		public string Type { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public DateTime? CloseDate { get; set; }
	}
}
