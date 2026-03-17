using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.BidDTO
{
    public class BidUpdateDto
    {
        [Required]
        public long BidID { get; set; }

        public DateTime? BidDate { get; set; }

        public decimal? TotalValue { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        [MaxLength(30)]
        public string? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
