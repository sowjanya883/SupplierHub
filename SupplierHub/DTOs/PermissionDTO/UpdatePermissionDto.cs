using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.PermissionDTO
{
    public class UpdatePermissionDto
    {
        [MaxLength(120)]
        public string? Code { get; set; }

        [MaxLength(150)]
        public string? PermissionName { get; set; }

        [MaxLength(30)]
        public string? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
