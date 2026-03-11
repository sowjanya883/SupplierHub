using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	public class BidLine
	{
		[Key]
		public int BidLineID { get; set; }

		[Required]
		public int BidID { get; set; }
		[ForeignKey("BidID")]
		public virtual Bid Bid { get; set; }

		public int RFxLineID { get; set; } 
		[ForeignKey("RFxLineID")]
		public virtual RFxLine RFxLine { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal UnitPrice { get; set; }

		public int LeadTimeDays { get; set; }

		public string Notes { get; set; }
	}
}
