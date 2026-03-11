using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
    /// <summary>
    /// Supplier scorecard snapshot for the period with rank and metrics.
    /// </summary>
    public class Scorecard
    {
        [Key]
        public int ScorecardID { get; set; }

        [Required]
        public int SupplierID { get; set; }

        /// <summary>YYYY-MM</summary>
        [Required, MinLength(7), MaxLength(7)]
        public string Period { get; set; } = default!;

        public string? MetricsJSON { get; set; }   // stores OTIF, NCR PPM, etc.

        public int? Rank { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}