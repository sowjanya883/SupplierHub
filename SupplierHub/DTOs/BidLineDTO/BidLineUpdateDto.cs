using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.BidLineDTO
{
    public class BidLineUpdateDto
    {
        [Required]
        public long BidLineID { get; set; }

        public decimal? UnitPrice { get; set; }

        public int? LeadTimeDays { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(30)]
        public string? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
