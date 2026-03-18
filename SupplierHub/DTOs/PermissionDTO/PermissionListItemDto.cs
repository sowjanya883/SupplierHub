using System;

namespace SupplierHub.DTOs.PermissionDTO
{
    public class PermissionListItemDto
    {
        public long PermissionID { get; set; }
        public string Code { get; set; } = default!;
        public string PermissionName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
    }
}
