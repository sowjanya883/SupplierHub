using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ContractDTO
{
	public class ContractDeleteDto
	{
		public long? ContractID { get; set; }

		public long? SupplierID { get; set; }

		public long? ItemID { get; set; }
	}
}