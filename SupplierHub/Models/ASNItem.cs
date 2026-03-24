using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class AsnItem
	{
		[Key]
		public long AsnItemID { get; set; }

		[Required]
		public long AsnID { get; set; }

		[Required]
		public long PoLineID { get; set; }

		public decimal? ShippedQty { get; set; }

		[MaxLength(100)]
		public string? LotBatch { get; set; }

		public string? SerialJson { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }


		[Required]
		public DateTime UpdatedOn { get; set; }

	}
}