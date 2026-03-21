using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierContactDTO
{
	public class SupplierContactUpdateDto : SupplierContactCreateDto
	{
		public long ContactID { get; set; }
	}
}