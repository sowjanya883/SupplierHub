using System;

namespace SupplierHub.DTOs.UserRoleDTO
{
    public class UserRoleDto
    {
        public long UserID { get; set; }
        public long RoleID { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
