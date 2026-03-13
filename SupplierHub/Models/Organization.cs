using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Organization
	{
		[Key]
		public long OrgID { get; set; }

		[Required, MaxLength(200)]
		public required string OrganizationName { get; set; }

		public string? AddressJson { get; set; }

		[MaxLength(50)]
		public string? TaxID { get; set; }

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