using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplierHub.Constants.Enum;

namespace SupplierHub.Models
{
	[Table("match_ref")]
	public class MatchRef
	{
		[Key]
		[Column("match_id")]
		public long MatchId { get; set; }

		[Required]
		[Column("invoice_id")]
		public long InvoiceId { get; set; }

		[Column("po_id")]
		public long? PoId { get; set; }

		[Column("grn_id")]
		public long? GrnId { get; set; }

		[Column("result")]
		[StringLength(20)]
		public string Result { get; set; } // Uses MatchResult Enum

		[Column("notes")]
		[StringLength(500)]
		public string Notes { get; set; }

		[Required]
		[Column("status")]
		[StringLength(30)]
		public MatchStatus Status { get; set; } = MatchStatus.Active;

		[Column("createdon")]
		public DateTime CreatedOn { get; set; } = DateTime.Now;

		[Column("updatedon")]
		public DateTime UpdatedOn { get; set; } = DateTime.Now;
	}
}