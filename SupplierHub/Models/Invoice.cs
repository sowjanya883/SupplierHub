using SupplierHub.Constants.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	[Table("invoice")]
	public class Invoice
	{
		[Key]
		[Column("invoice_id")]
		public long InvoiceId { get; set; }

		[Required]
		[Column("supplier_id")]
		public long SupplierId { get; set; }

		[Column("po_id")]
		public long? PoId { get; set; }

		[ForeignKey("PoId")]
		public virtual PurchaseOrder PurchaseOrder { get; set; }

		[Column("invoice_no")]
		[StringLength(100)]
		public string InvoiceNo { get; set; }

		[Column("invoice_date", TypeName = "date")]
		public DateTime? InvoiceDate { get; set; }

		[Column("currency")]
		[StringLength(10)]
		public string Currency { get; set; }

		[Column("total_amount", TypeName = "decimal(18,2)")]
		public decimal TotalAmount { get; set; }

		[Column("tax_json")]
		public string TaxJson { get; set; } // JSON stored as string in C#

		[Required]
		[Column("status")]
		[StringLength(30)]
		public InvoiceStatus Status { get; set; } = InvoiceStatus.Submitted;

		[Column("createdon")]
		public DateTime CreatedOn { get; set; } = DateTime.Now;

		[Column("updatedon")]
		public DateTime UpdatedOn { get; set; } = DateTime.Now;

		public virtual ICollection<InvoiceLine> InvoiceLines { get; set; }
	}
}