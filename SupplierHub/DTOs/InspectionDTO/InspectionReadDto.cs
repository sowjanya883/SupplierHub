using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Dtos.InspectionDTO
{
    public class InspectionReadDto
    {
        public long InspID { get; set; }
        public long GrnItemID { get; set; }
        public string? Result { get; set; }
        public long? InspectorID { get; set; }
        public DateTime? InspDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
