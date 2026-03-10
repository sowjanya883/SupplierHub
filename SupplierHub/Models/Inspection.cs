using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
    /// <summary>
    /// Quality inspection record for a GRN item; Pass/Fail with findings.
    /// </summary>
    public class Inspection
    {
        [Key]
        public int InspID { get; set; }

        [Required]
        public int GRNItemID { get; set; }

        /// <summary>Pass or Fail</summary>
        [Required, MaxLength(10)]
        public string Result { get; set; } = default!;

        public string? FindingsJSON { get; set; }

        [Required]
        public int InspectorID { get; set; }     // simple numeric user ID for now

        [Required]
        public DateTime InspDate { get; set; } = DateTime.UtcNow;
    }
}