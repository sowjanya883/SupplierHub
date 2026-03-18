using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RoleDTO
{
    public class UpdateRoleDto
    {
        [MaxLength(100)]
        public string? RoleName { get; set; }

        [MaxLength(30)]
        public string? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
