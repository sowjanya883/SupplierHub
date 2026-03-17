using System;
using System.ComponentModel.DataAnnotations;


namespace SupplierHub.DTOs.GrnRefDTO
{
    public class GrnReadDto
    {
        public long GrnID { get; set; }
        public long PoID { get; set; }
        public long? AsnID { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public long? ReceivedBy { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
