using System.ComponentModel.DataAnnotations;


namespace SupplierHub.DTOs.ComplianceDocDTO
{
	public class ComplianceDocUpdateDto : ComplianceDocCreateDto
	{
		public long DocID { get; set; }
	}
}