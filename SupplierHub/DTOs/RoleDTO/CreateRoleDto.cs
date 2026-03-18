using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RoleDTO
{
    public class CreateRoleDto
    {
        [Required, MaxLength(100)]
        public string RoleName { get; set; } = default!;

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Active";
    }
}
