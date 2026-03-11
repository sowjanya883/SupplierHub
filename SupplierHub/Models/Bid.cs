using SupplierHub.Constants.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	public class Bid
	{
		[Key]
		public int BidID { get; set; }

		[Required]
		public int RFxID { get; set; }
		[ForeignKey("RFxID")]
		public virtual RFxEvent RFxEvent { get; set; }

		public int SupplierId { get; set; }
		[ForeignKey(nameof(SupplierId))]

		public virtual Supplier Supplier { get; set; }

		public DateTime BidDate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal TotalValue { get; set; }

		[StringLength(3)]
		public string Currency { get; set; } 

		[StringLength(20)]
		public BidStatus Status { get; set; } 

		public virtual ICollection<BidLine> BidLines { get; set; }
	}
}
