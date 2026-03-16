using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Dtos
{
    public class InspectionUpdateDto
    {
        [Required]
        public long GrnItemID { get; set; }

        [MaxLength(10)]
        public string? Result { get; set; }

        public long? InspectorID { get; set; }

        public DateTime? InspDate { get; set; }

        [Required, MaxLength(30)]
        public required string Status { get; set; }
    }
}