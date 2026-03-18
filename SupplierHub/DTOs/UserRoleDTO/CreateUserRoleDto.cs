using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.UserRoleDTO
{
    public class CreateUserRoleDto
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        public long RoleID { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Active";
    }
}
