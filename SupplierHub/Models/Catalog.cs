using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Catalog
	{
		[Key]
		public long CatalogID { get; set; }

		[Required]
		public long SupplierID { get; set; }

		[Required, MaxLength(200)]
		public required string CatalogName { get; set; }

		public DateTime? ValidFrom { get; set; }

		public DateTime? ValidTo { get; set; }

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