using System;

namespace SupplierHub.DTOs.UserRoleDTO
{
    public class UserRoleListItemDto
    {
        public long UserID { get; set; }
        public long RoleID { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
    }
}
