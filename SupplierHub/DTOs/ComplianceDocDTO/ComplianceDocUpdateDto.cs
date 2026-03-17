using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ComplianceDocDTO
{
	public class ComplianceDocUpdateDto : ComplianceDocCreateDto
	{
		[Required]
		public long DocID { get; init; }
	}
}