using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
    /// <summary>
    /// Item line received within a GRN, with quantity breakdown and variance reason.
    /// </summary>
    public class GRNItemRef
    {
        [Key]
        public int GRNItemID { get; set; }

        [Required]
        public int GRNID { get; set; }       // FK to GRNRef

        [Required]
        public int POLineID { get; set; }    // FK to PO line (can add table later)

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999999999999.99)]
        public decimal ReceivedQty { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999999999999.99)]
        public decimal AcceptedQty { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999999999999.99)]
        public decimal RejectedQty { get; set; }

        [MaxLength(200)]
        public string? Reason { get; set; }  // damage/shortage/etc.
    }
}