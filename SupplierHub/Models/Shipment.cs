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

		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}