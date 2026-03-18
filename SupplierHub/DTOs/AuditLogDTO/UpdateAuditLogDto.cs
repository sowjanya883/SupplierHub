using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.AuditLogDTO
{
    public class UpdateAuditLogDto
    {
        public long? UserID { get; set; }

        [MaxLength(100)]
        public string? Action { get; set; }

        [MaxLength(200)]
        public string? Resource { get; set; }

        public string? Metadata { get; set; }

        public DateTime? Timestamp { get; set; }

        [MaxLength(30)]
        public string? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
