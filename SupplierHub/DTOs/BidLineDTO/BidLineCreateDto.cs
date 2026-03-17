using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.BidLineDTO
{
    public class BidLineCreateDto
    {
        [Required]
        public long BidID { get; set; }

        [Required]
        public long RfxLineID { get; set; }

        public decimal? UnitPrice { get; set; }

        public int? LeadTimeDays { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = string.Empty;
    }
}
