using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RolePermissionDTO
{
    public class UpdateRolePermissionDto
    {
        [MaxLength(30)]
        public string? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
