using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RequisitionDto
{
	public class RequisitionCreateDto
	{
		[Required]
		public long RequesterID { get; set; }

		[Required]
		public long OrgID { get; set; }

		[MaxLength(50)]
		public string? CostCenter { get; set; }

		[MaxLength(500)]
		public string? Justification { get; set; }

		public DateTime? RequestedDate { get; set; }

		public DateTime? NeededByDate { get; set; }
		[Required]
		public long RequesterUserID { get; set; }

		// Status is often omitted here if it defaults to 'DRAFT' in the DB
	}
}