using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	[Table("Shipment")]
	public class Shipment
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ShipmentID { get; set; }

		[Required]
		[ForeignKey(nameof(PurchaseOrder))]
		public long POID { get; set; }

		[Required]
		[ForeignKey(nameof(Supplier))]
		public long SupplierID { get; set; }

		[Required]
		public DateTime ShipDate { get; set; }

		[StringLength(100)]
		public string Carrier { get; set; }

		[StringLength(100)]
		public string TrackingNo { get; set; }

		[Required]
		[StringLength(20)]
		public string Status { get; set; }

		// Navigation Properties
		public virtual PurchaseOrder PurchaseOrder { get; set; }

		public virtual Supplier Supplier { get; set; }

		public virtual ICollection<ASN> ASNs { get; set; } = new List<ASN>();
	}
}