using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierDTO
{
	public class UpdateSupplierDto : SupplierCreateDto
	{
		public long SupplierID { get; set; }
	}
}