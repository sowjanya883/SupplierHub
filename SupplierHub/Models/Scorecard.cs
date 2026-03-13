using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Scorecard
	{
		[Key]
		public long ScorecardID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		[Required, MaxLength(20)]
		public required string Period { get; set; }

		public string? MetricsJson { get; set; }

		public int? Rank { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

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