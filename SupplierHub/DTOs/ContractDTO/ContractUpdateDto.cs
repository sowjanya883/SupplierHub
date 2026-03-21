using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ContractDTO
{
	public class ContractUpdateDto : ContractCreateDto
	{
		public long ContractID { get; set; }
	}
}