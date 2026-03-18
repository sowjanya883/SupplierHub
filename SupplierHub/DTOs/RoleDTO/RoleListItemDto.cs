using System;

namespace SupplierHub.DTOs.RoleDTO
{
    public class RoleListItemDto
    {
        public long RoleID { get; set; }
        public string RoleName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
    }
}
