namespace SupplierHub.DTOs.SupplierDTO
{
	public class GetSupplierByIdDto : UpdateSupplierDto
	{
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}