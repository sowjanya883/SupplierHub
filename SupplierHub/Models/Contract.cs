using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Contract
	{
		[Key]
		public long ContractID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		public long? ItemID { get; set; }

		public string? TermsJson { get; set; }

		public decimal? Rate { get; set; }

		[MaxLength(10)]
		public string? Currency { get; set; }

		public DateTime? ValidFrom { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		public DateTime? ValidTo { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}