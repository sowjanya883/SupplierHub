using System;
using SupplierHub.DTOs.SupplierDTO;

namespace SupplierHub.DTOs.SupplierDTO
{

	// GET/Response DTO with identity and auditing info.
	// Inherits common fields from CreateSupplierDto.
	public class SupplierResponseDto : SupplierCreateDto
	{
		public long SupplierID { get; init; }
		public DateTime CreatedOn { get; init; }
		public DateTime UpdatedOn { get; init; }
		public bool IsDeleted { get; init; }
	}
}