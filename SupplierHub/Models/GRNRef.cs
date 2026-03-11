using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
    /// <summary>
    /// GRN (Goods Receipt Note) header recorded against a PO/ASN.
    /// </summary>
    public class GRNRef
    {
        [Key]
        public int GRNID { get; set; }

        [Required]
        public int POID { get; set; }        // Reference to PO (foreign PO table can be added later)

        public int? ASNID { get; set; }      // Optional reference to ASN

        [Required]
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(150)]
        public string ReceivedBy { get; set; } = default!;

        /// <summary>Open/Posted (keep as string for now; can change to enum later)</summary>
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Open";

        // Navigation
        public ICollection<GRNItemRef> Items { get; set; } = new List<GRNItemRef>();
    }
}