using System;

namespace SupplierHub.DTOs.AuditLogDTO
{
    public class AuditLogListItemDto
    {
        public long AuditID { get; set; }
        public long? UserID { get; set; }
        public string Action { get; set; } = default!;
        public string Resource { get; set; } = default!;
        public DateTime Timestamp { get; set; }
    }
}
