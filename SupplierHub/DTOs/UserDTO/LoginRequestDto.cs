using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.UserDTO
{
    public class LoginRequestDto
    {
        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; } = default!;

        [Required, MinLength(6), MaxLength(255)]
        public string Password { get; set; } = default!;
    }
}
