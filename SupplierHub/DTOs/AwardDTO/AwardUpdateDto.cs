using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.AwardDTO
{
    public class AwardUpdateDto
    {
        [Required]
        public long AwardID { get; set; }

        public DateTime? AwardDate { get; set; }

        public decimal? AwardValue { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(30)]
        public string? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
