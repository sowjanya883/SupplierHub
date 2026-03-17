using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RequisitionDto
{
	public class RequisitionUpdateDto
	{
		[MaxLength(50)]
		public string? CostCenter { get; set; }

		[MaxLength(500)]
		public string? Justification { get; set; }

		public DateTime? RequestedDate { get; set; }

		public DateTime? NeededByDate { get; set; }

		[Required, MaxLength(30)]
		public string Status { get; set; } = null!;
	}
}