namespace SupplierHub.DTOs.ComplianceDocDTO
{
	public class ComplianceDocGetByIdDto : ComplianceDocUpdateDto
	{
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}