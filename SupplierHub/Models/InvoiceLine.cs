using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class InvoiceLine
	{
		[Key]
		public long InvLineID { get; set; }

		[Required]
		public long InvoiceID { get; set; }

		public long? PoLineID { get; set; }

		public decimal? Qty { get; set; }

		public decimal? UnitPrice { get; set; }

		public decimal? LineTotal { get; set; }

		public string? TaxJson { get; set; }

		[MaxLength(20)]
		public string? MatchStatus { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}