using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
    /// <summary>
    /// Roll-up KPIs per supplier per period (YYYY-MM).
    /// </summary>
    public class SupplierKPI
    {
        [Key]
        public int KPIID { get; set; }

        [Required]
        public int SupplierID { get; set; }

        /// <summary>YYYY-MM</summary>
        [Required, MinLength(7), MaxLength(7)]
        public string Period { get; set; } = default!;

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal? OTIFPct { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000000000000000.00)]
        public decimal? NCRRatePPM { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 1000000000.00)]
        public decimal? AvgAckTimeHrs { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal? PriceAdherencePct { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Score { get; set; }

        [Required]
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    }
}