using SupplierHub.Constants.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	[Table("invoice_line")]
	public class InvoiceLine
	{
		[Key]
		[Column("inv_line_id")]
		public long InvLineId { get; set; }

		[Required]
		[Column("invoice_id")]
		public long InvoiceId { get; set; }

		[ForeignKey("InvoiceId")]
		public virtual Invoice Invoice { get; set; }

		[Column("po_line_id")]
		public long? PoLineId { get; set; }

		[ForeignKey("PoLineId")]
		public virtual POLine POLine { get; set; }

		[Column("qty", TypeName = "decimal(18,3)")]
		public decimal Qty { get; set; }

		[Column("unit_price", TypeName = "decimal(18,4)")]
		public decimal UnitPrice { get; set; }

		[Column("line_total", TypeName = "decimal(18,2)")]
		public decimal LineTotal { get; set; }

		[Column("tax_json")]
		public string TaxJson { get; set; }

		[Column("match_status")]
		[StringLength(20)]
		public MatchStatus MatchStatus { get; set; }

		[Column("createdon")]
		public DateTime CreatedOn { get; set; } = DateTime.Now;

		[Column("updatedon")]
		public DateTime UpdatedOn { get; set; } = DateTime.Now;
	}
}