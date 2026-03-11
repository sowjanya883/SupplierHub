using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("supplier")]

	// Indexes for fast searching
	[Index(nameof(Status), Name = "idx_supplier_status")]
	[Index(nameof(UpdatedOn), Name = "idx_supplier_updatedon")]

	public class Supplier
	{
		[Key]
		public long SupplierId { get; set; }

		[Required, MaxLength(200)]
		public string LegalName { get; set; } = default!;

		[MaxLength(50)]
		public string? DunsOrRegNo { get; set; }

		[MaxLength(50)]
		public string? TaxId { get; set; }

		// Store JSON via Fluent API HasColumnType("json") in DbContext
		public string? BankInfoJson { get; set; }

	
		public long? PrimaryContactId { get; set; } // FK → SupplierContact.ContactId (optional)

		[Required]
		public SupplierStatus Status { get; set; } = SupplierStatus.Active;

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedOn { get; set; }

		// Navigation
		[ForeignKey(nameof(PrimaryContactId))]
		public virtual SupplierContact? PrimaryContact { get; set; }

		public virtual ICollection<SupplierContact> Contacts { get; set; } = new List<SupplierContact>();
		public virtual ICollection<ComplianceDoc> ComplianceDocs { get; set; } = new List<ComplianceDoc>();
		public virtual ICollection<SupplierRisk> Risks { get; set; } = new List<SupplierRisk>();
		public virtual ICollection<RFxInvite> RFxInvites { get; set; }
		public virtual ICollection<Award> Awards { get; set; }
		public virtual ICollection<Bid> Bids { get; set; }
	}
}