using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Supplier
	{
		[Key]
		public long SupplierID { get; set; }

		[Required, MaxLength(200)]
		public string LegalName { get; set; }

		[MaxLength(50)]
		public string? DunsOrRegNo { get; set; }

		[MaxLength(50)]
		public string? TaxID { get; set; }

		public string? BankInfoJson { get; set; }

		public long? PrimaryContactID { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}