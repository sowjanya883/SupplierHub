using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.AwardDTO
{
    public class AwardCreateDto
    {
        [Required]
        public long RfxID { get; set; }

        [Required]
        public long SupplierID { get; set; }

        public DateTime? AwardDate { get; set; }

        public decimal? AwardValue { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = string.Empty;
    }
}
