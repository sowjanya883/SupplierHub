using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class SupplierKpi
	{
		[Key]
		public long KpiID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		[Required, MaxLength(20)]
		public required string Period { get; set; }

		public decimal? NcrRatePpm { get; set; }

		public decimal? AvgAckTimeHrs { get; set; }

		public decimal? PriceAdherencePct { get; set; }

		public decimal? Score { get; set; }

		public DateTime? GeneratedDate { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}