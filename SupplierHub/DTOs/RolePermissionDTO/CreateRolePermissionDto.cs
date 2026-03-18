using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RolePermissionDTO
{
    public class CreateRolePermissionDto
    {
        [Required]
        public long RoleID { get; set; }

        [Required]
        public long PermissionID { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Active";
    }
}
