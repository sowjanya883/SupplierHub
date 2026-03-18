using System;

namespace SupplierHub.DTOs.RoleDTO
{
    public class RoleDto
    {
        public long RoleID { get; set; }
        public string RoleName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
