using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierDTO
{

	// Update (PUT/PATCH) payload
	// Inherits common fields from CreateSupplierDto
	public class UpdateSupplierDto : CreateSupplierDto
	{
		[Required]
		public long SupplierID { get; init; }
	}
}