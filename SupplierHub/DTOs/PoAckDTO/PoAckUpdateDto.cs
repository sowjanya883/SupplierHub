using System;
using System.ComponentModel.DataAnnotations;
using SupplierHub.Constants.Enum;

namespace SupplierHub.DTOs.PoAckDTO
{
    public class PoAckUpdateDto
    {
        [Required]
        public long PocfmID { get; set; }

        [Required]
        public long PoId { get; set; }

        [Required]
        public long SupplierId { get; set; }

        public DateTime? AcknowledgeDate { get; set; }

        public PoAckDecision? Decision { get; set; }

        [StringLength(500)]
        public string? CounterNotes { get; set; }

        public PoAckStatus? Status { get; set; }
    }
}
