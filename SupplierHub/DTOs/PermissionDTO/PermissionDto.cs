using System;

namespace SupplierHub.DTOs.PermissionDTO
{
    public class PermissionDto
    {
        public long PermissionID { get; set; }
        public string Code { get; set; } = default!;
        public string PermissionName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
