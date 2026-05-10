namespace SupplierHub.DTOs.RfxEventDTO
{
	public class RFxEventReadDto
	{
		public long RfxID { get; set; }
		public string Type { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public long? CategoryID { get; set; }
		public long? CreatedBy { get; set; }
		public DateTime? OpenDate { get; set; }
		public DateTime? CloseDate { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}
