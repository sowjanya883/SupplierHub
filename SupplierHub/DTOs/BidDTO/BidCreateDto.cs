using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.BidDTO
{
    public class BidCreateDto
    {
        [Required]
        public long RfxID { get; set; }

        [Required]
        public long SupplierID { get; set; }

        public DateTime? BidDate { get; set; }

        public decimal? TotalValue { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = string.Empty;
    }
}
