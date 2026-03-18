using System;

namespace SupplierHub.DTOs.NotificationDTO
{
    public class NotificationListItemDto
    {
        public long NotificationID { get; set; }
        public long UserID { get; set; }
        public string Message { get; set; } = default!;
        public string? Category { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
    }
}
