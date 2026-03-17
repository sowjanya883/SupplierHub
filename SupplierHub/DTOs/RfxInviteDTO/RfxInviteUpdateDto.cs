using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.RfxInviteDTO
{
    public class RfxInviteUpdateDto
    {
        [Required]
        public long InviteID { get; set; }

        public DateTime? InvitedDate { get; set; }

        [MaxLength(30)]
        public string? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
