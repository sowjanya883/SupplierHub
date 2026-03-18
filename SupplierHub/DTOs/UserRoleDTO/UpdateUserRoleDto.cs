using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.UserRoleDTO
{
    public class UpdateUserRoleDto
    {
        [MaxLength(30)]
        public string? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
