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

<<<<<<< HEAD
		[StringLength(int.MaxValue)]
		public string SerialJSON { get; set; }

		[StringLength(500)]
		public string Notes { get; set; }
		public bool IsDeleted { get; set; }  // default -> false

		// Navigation Properties
		public virtual ASN ASN { get; set; }

		public virtual POLine POLine { get; set; }
=======
		[Required]
		public DateTime UpdatedOn { get; set; }
>>>>>>> f5b24b19b20cc4f606a8ea7902667aadcbaffb0f
	}
}