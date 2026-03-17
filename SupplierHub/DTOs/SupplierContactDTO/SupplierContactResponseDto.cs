using System;

namespace SupplierHub.DTOs.SupplierContactDTO
{
	public class SupplierContactResponseDto : SupplierContactCreateDto
	{
		public long ContactID { get; init; }
		public string? Status { get; init; }
		public DateTime CreatedOn { get; init; }
		public DateTime UpdatedOn { get; init; }
		public bool IsDeleted { get; init; }
	}
}