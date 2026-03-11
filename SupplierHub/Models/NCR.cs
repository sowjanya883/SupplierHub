using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
    /// <summary>
    /// Non-Conformance Report created when inspection fails or quality issue found.
    /// </summary>
    public class NCR
    {
        [Key]
        public int NCRID { get; set; }

        [Required]
        public int GRNItemID { get; set; }

        [Required, MaxLength(100)]
        public string DefectType { get; set; } = default!;  // e.g., "InspectionFail"

        /// <summary>Minor/Major/Critical</summary>
        [Required, MaxLength(20)]
        public string Severity { get; set; } = default!;

        /// <summary>UseAsIs/Rework/Reject/Return</summary>
        [Required, MaxLength(20)]
        public string Disposition { get; set; } = default!;

        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>Open/Closed</summary>
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Open";
    }
}