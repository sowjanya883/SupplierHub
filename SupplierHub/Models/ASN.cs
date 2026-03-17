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
<<<<<<< HEAD
		[StringLength(20)]
		public string Status { get; set; }
		public bool IsDeleted { get; set; }  // default -> false
=======
		public bool IsDeleted { get; set; }
>>>>>>> f5b24b19b20cc4f606a8ea7902667aadcbaffb0f

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}