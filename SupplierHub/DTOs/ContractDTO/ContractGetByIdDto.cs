namespace SupplierHub.DTOs.ContractDTO
{
	public class ContractGetByIdDto : ContractUpdateDto
	{
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}