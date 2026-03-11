using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("supplier_contact")]
	[Index(nameof(SupplierId), Name = "idx_contact_supplier")]
	[Index(nameof(Email), Name = "idx_contact_email")]
	public class SupplierContact
	{
		[Key]
		public long ContactId { get; set; }

		[Required]
		public long SupplierId { get; set; } // FK → Supplier.SupplierId

		[Required, MaxLength(150)]
		public string Name { get; set; } = default!;

		[MaxLength(150)]
		public string? Email { get; set; }

		[MaxLength(30)]
		public string? Phone { get; set; }

		[MaxLength(100)]
		public string? Role { get; set; }

		[Required]
		public ContactStatus Status { get; set; } = ContactStatus.Active;

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedOn { get; set; }

		// Navigation
		[ForeignKey(nameof(SupplierId))]
		public virtual Supplier Supplier { get; set; } = default!;
	}
}