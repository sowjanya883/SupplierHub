using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	[Table("ASNItem")]
	public class ASNItem
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ASNItemID { get; set; }

		[Required]
		[ForeignKey(nameof(ASN))]
		public long ASNID { get; set; }

		[Required]
		[ForeignKey(nameof(POLine))]
		public long POLineID { get; set; }

		[Required]
		public decimal ShippedQty { get; set; }

		[StringLength(100)]
		public string LotBatch { get; set; }

		[StringLength(int.MaxValue)]
		public string SerialJSON { get; set; }

		[StringLength(500)]
		public string Notes { get; set; }

		// Navigation Properties
		public virtual ASN ASN { get; set; }

		public virtual POLine POLine { get; set; }
	}
}