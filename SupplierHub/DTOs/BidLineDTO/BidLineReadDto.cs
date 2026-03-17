using System;

namespace SupplierHub.DTOs.BidLineDTO
{
    public class BidLineReadDto
    {
        public long BidLineID { get; set; }
        public long BidID { get; set; }
        public long RfxLineID { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? LeadTimeDays { get; set; }
        public string? Currency { get; set; }
        public bool IsDeleted { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
