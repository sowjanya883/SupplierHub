using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RfxInviteDTO
{
    public class RfxInviteCreateDto
    {
        [Required]
        public long RfxID { get; set; }

        [Required]
        public long SupplierID { get; set; }

        public DateTime? InvitedDate { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = string.Empty;
    }
}
