using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Asn
	{
		[Key]
		public long AsnID { get; set; }

		[Required]
		public long ShipmentID { get; set; }

		[MaxLength(100)]
		public string? AsnNo { get; set; }

		public DateTime? CreatedDate { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]

		public bool IsDeleted { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}