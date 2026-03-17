using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierContactDTO
{
	public class SupplierContactCreateDto
	{
		[Required]
		public long SupplierID { get; init; }

		[Required, MaxLength(150)]
		public required string SupplierName { get; init; }

		[MaxLength(150)]
		public string? Email { get; init; }

		[MaxLength(30)]
		public string? Phone { get; init; }

		[MaxLength(100)]
		public string? Role { get; init; }
	}
}