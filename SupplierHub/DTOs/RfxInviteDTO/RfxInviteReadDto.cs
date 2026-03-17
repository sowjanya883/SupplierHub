using System;

namespace SupplierHub.DTOs.RfxInviteDTO
{
    public class RfxInviteReadDto
    {
        public long InviteID { get; set; }
        public long RfxID { get; set; }
        public long SupplierID { get; set; }
        public DateTime? InvitedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
