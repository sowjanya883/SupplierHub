using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	[Table("PRLine")]
	public class PRLine
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long PRLineID { get; set; }

		[Required]
		[ForeignKey(nameof(Requisition))]
		public long PRID { get; set; }

		[Required]
		[ForeignKey(nameof(Item))]
		public long ItemID { get; set; }

		[StringLength(int.MaxValue)]
		public string Description { get; set; }

		[Required]
		public decimal Qty { get; set; }

		[StringLength(20)]
		public string UoM { get; set; }

		[Required]
		public decimal TargetPrice { get; set; }

		[StringLength(3)]
		public string Currency { get; set; } = "USD";

		[ForeignKey(nameof(SupplierPreferred))]
		public long? SupplierPreferredID { get; set; }

		[StringLength(500)]
		public string Notes { get; set; }

		// Navigation Properties
		public virtual Requisition Requisition { get; set; }

		public virtual Item Item { get; set; }

		public virtual Supplier SupplierPreferred { get; set; }
	}
}