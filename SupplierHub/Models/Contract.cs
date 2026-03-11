using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("contract")]
	[Index(nameof(SupplierId), Name = "idx_contract_supplier")]
	public class Contract
	{
		[Key]
		public long ContractId { get; set; }

		[Required]
		public long SupplierId { get; set; }

		public long? ItemId { get; set; }

		// JSON
		public string? TermsJson { get; set; }

		public decimal? Rate { get; set; } // decimal(18,4)

		[MaxLength(10)]
		public string? Currency { get; set; }

		public DateOnly? ValidFrom { get; set; }

		public DateOnly? ValidTo { get; set; }

		[Required]
		public ContractStatus Status { get; set; } = ContractStatus.Active;

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedOn { get; set; }

		// Navigation
		[ForeignKey(nameof(SupplierId))]
		public Supplier Supplier { get; set; } = default!;

		[ForeignKey(nameof(ItemId))]
		public Item? Item { get; set; }
	}
}