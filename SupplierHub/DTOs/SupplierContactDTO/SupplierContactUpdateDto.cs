using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierContactDTO
{
	public class SupplierContactUpdateDto : SupplierContactCreateDto
	{
		[Required]
		public long ContactID { get; init; }
	}
}