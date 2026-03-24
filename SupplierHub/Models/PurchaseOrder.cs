using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class PurchaseOrder
	{
		[Key]
		public long PoID { get; set; }

		[Required]
		public long OrgID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		public DateTime? PoDate { get; set; }

		[MaxLength(10)]
		public string? Currency { get; set; }

		[MaxLength(50)]
		public string? Incoterms { get; set; }

		[MaxLength(100)]
		public string? PaymentTerms { get; set; }

		[Required, MaxLength(50)]
		public required string? Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}