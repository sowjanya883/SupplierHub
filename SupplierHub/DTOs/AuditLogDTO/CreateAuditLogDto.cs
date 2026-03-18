using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.AuditLogDTO
{
    public class CreateAuditLogDto
    {
        public long? UserID { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; } = default!;

        [Required, MaxLength(200)]
        public string Resource { get; set; } = default!;

        public string? Metadata { get; set; }

        public DateTime? Timestamp { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Active";
    }
}
