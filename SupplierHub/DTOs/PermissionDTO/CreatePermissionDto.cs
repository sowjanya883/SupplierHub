using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.PermissionDTO
{
    public class CreatePermissionDto
    {
        [Required, MaxLength(120)]
        public string Code { get; set; } = default!;

        [Required, MaxLength(150)]
        public string PermissionName { get; set; } = default!;

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Active";
    }
}
