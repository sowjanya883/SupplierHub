using System;

namespace SupplierHub.DTOs.BidDTO
{
    public class BidReadDto
    {
        public long BidID { get; set; }
        public long RfxID { get; set; }
        public long SupplierID { get; set; }
        public DateTime? BidDate { get; set; }
        public decimal? TotalValue { get; set; }
        public string? Currency { get; set; }
        public bool IsDeleted { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
