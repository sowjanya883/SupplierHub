using SupplierHub.Constants.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	public class RFxLine
	{
		[Key]
		public int RFxLineID { get; set; }

		[Required]
		public int RFxID { get; set; }
		[ForeignKey("RFxID")]
		public virtual RFxEvent RFxEvent { get; set; }

		public int ItemID { get; set; }
		[ForeignKey(nameof(ItemID))]
		public virtual Item Item { get; set; }

		[Required]
		[Column(TypeName = "decimal(18,2)")]
		public decimal Qty { get; set; }

		[Required]
		[StringLength(10)]
		public UOM UoM { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal? TargetPrice { get; set; }

		public string Notes { get; set; }

		public ICollection<BidLine> BidLines { get; set; }

	}
}
