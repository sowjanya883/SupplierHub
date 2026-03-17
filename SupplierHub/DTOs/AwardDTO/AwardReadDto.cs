using System;

namespace SupplierHub.DTOs.AwardDTO
{
    public class AwardReadDto
    {
        public long AwardID { get; set; }
        public long RfxID { get; set; }
        public long SupplierID { get; set; }
        public DateTime? AwardDate { get; set; }
        public decimal? AwardValue { get; set; }
        public string? Currency { get; set; }
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
