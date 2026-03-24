using System;
using SupplierHub.Constants.Enum;

namespace SupplierHub.DTOs.PoAckDTO
{
    public class PoAckResponseDto
    {
        public long PocfmID { get; set; }
        public long PoId { get; set; }
        public long SupplierId { get; set; }
        public DateTime? AcknowledgeDate { get; set; }
        public PoAckDecision? Decision { get; set; }
        public string? CounterNotes { get; set; }
        public PoAckStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
