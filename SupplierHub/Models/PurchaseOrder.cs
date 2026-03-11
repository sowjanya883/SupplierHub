using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplierHub.Constants.Enum;
using SupplierHub.Constants.Enums; // Add this using statement!

namespace SupplierHub.Models
{
    [Table("purchase_order")]
    public class PurchaseOrder
    {
        [Key]
        [Column("po_id")]
        public long PoId { get; set; }

        [Required]
        [Column("org_id")]
        public long OrgId { get; set; }

        [Required]
        [Column("supplier_id")]
        public long SupplierId { get; set; }

        [Column("po_date", TypeName = "date")]
        public DateTime PoDate { get; set; } = DateTime.Now;

        [Column("currency")]
        [StringLength(10)]
        public string ? Currency { get; set; }

        [Column("incoterms")]
        [StringLength(50)]
        public string ? Incoterms { get; set; }

        [Column("payment_terms")]
        [StringLength(100)]
        public string ? PaymentTerms { get; set; }
        [Required]
        [Column("status")]
        [StringLength(50)]
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.OPEN;

        [Column("createdon")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Column("updatedon")]
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}