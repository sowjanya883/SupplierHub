using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("supplier")]
	public class Supplier
	{
		[Key]
		public long SupplierId { get; set; }

		[Required, MaxLength(200)]
		public string LegalName { get; set; }

		[MaxLength(50)]
		public string? DunsOrRegNo { get; set; }

		[MaxLength(50)]
		public string? TaxId { get; set; }

		public string? BankInfoJson { get; set; }

		public long? PrimaryContactId { get; set; }

		[Required]
		public SupplierStatus Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public bool IsDeleted { get; set; }  // default -> false

		// Navigation
		[ForeignKey(nameof(PrimaryContactId))]
		public virtual SupplierContact? PrimaryContact { get; set; }

		public virtual ICollection<SupplierContact> Contacts { get; set; }
		public virtual ICollection<ComplianceDoc> ComplianceDocs { get; set; }
		public virtual ICollection<SupplierRisk> Risks { get; set; }
		public virtual ICollection<Catalog> Catalogs { get; set; }

		public virtual ICollection<Bid> Bids { get; set; }

		public virtual ICollection<RFxInvite> RFxInvites { get; set; }
		public virtual ICollection<Award> Awards { get; set; }
	}
}