using System;

namespace SupplierHub.DTOs.RolePermissionDTO
{
    public class RolePermissionListItemDto
    {
        public long RoleID { get; set; }
        public long PermissionID { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
    }
}
