using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Invoice
	{
		[Key]
		public long InvoiceID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		public long? PoID { get; set; }

		[MaxLength(100)]
		public string? InvoiceNo { get; set; }

		public DateTime? InvoiceDate { get; set; }

		[MaxLength(10)]
		public string? Currency { get; set; }

		public decimal? TotalAmount { get; set; }

		public string? TaxJson { get; set; }

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