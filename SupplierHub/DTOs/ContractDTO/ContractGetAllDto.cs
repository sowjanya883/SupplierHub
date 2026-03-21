using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ContractDTO
{
	public class ContractGetAllDto
	{
		public long ContractID { get; set; }

		public long SupplierID { get; set; }

		public decimal? Rate { get; set; }

		public required string Status { get; set; }
	}
}