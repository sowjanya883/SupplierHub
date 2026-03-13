using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class Category
	{
		[Key]
		public long CategoryID { get; set; }

		public long? ParentCategoryID { get; set; }

		[Required, MaxLength(200)]
		public required string CategoryName { get; set; }

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