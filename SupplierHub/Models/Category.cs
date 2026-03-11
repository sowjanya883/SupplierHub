using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("category")]
	[Index(nameof(Status), Name = "idx_category_status")]
	[Index(nameof(UpdatedOn), Name = "idx_category_updatedon")]
	public class Category
	{
		[Key]
		public long CategoryId { get; set; }

		public long? ParentCategoryId { get; set; } // Self-reference

		[Required, MaxLength(200)]
		public string Name { get; set; } = default!;

		[Required]
		public CategoryStatus Status { get; set; } = CategoryStatus.Active;

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedOn { get; set; }



		// Navigation
		[ForeignKey(nameof(ParentCategoryId))]
		public Category? ParentCategory { get; set; }

		public ICollection<Category> SubCategories { get; set; } = new List<Category>();

		public ICollection<Item> Items { get; set; } = new List<Item>();

		public ICollection<RFxEvent> RFxEvents { get; set; }

	}
}