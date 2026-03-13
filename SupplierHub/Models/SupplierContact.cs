using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class SupplierContact
	{
		[Key]
		public long ContactID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		[Required, MaxLength(150)]
		public required string SupplierName { get; set; }

		[MaxLength(150)]
		public string? Email { get; set; }

		[MaxLength(30)]
		public string? Phone { get; set; }

		[MaxLength(100)]
		public string? Role { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}