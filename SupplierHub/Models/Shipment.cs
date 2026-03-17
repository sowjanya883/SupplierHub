using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Shipment
	{
		[Key]
		public long ShipmentID { get; set; }

		[Required]
		public long PoID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		public DateTime? ShipDate { get; set; }

		[MaxLength(100)]
		public string? Carrier { get; set; }

		[MaxLength(100)]
		public string? TrackingNo { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
<<<<<<< HEAD
		[StringLength(20)]
		public string Status { get; set; }
		public bool IsDeleted { get; set; }  // default -> false
=======
		public DateTime CreatedOn { get; set; }
>>>>>>> f5b24b19b20cc4f606a8ea7902667aadcbaffb0f

		[Required]
		public DateTime UpdatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}