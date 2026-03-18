using System;

namespace SupplierHub.DTOs.AuditLogDTO
{
    public class AuditLogDto
    {
        public long AuditID { get; set; }
        public long? UserID { get; set; }
        public string Action { get; set; } = default!;
        public string Resource { get; set; } = default!;
        public string? Metadata { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
