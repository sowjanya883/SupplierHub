using SupplierHub.Constants.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	public class Award
	{
		[Key]
		public int AwardID { get; set; }

		[Required]
		public int RFxID { get; set; }
		[ForeignKey("RFxID")]
		public virtual RFxEvent RFxEvent { get; set; }

		public int SupplierId { get; set; }
		[ForeignKey(nameof(SupplierId))] 
	    public virtual Supplier Supplier { get; set; }
		

		public DateTime AwardDate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal AwardValue { get; set; }

		public string Notes { get; set; }

		public AwardStatus Status { get; set; }

		public bool IsDeleted { get; set; } = false;
	}
}
