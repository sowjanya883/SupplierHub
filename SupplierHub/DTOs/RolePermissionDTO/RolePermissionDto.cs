using System;

namespace SupplierHub.DTOs.RolePermissionDTO
{
    public class RolePermissionDto
    {
        public long RoleID { get; set; }
        public long PermissionID { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
