using SupplierHub.Constants.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	public class RFxInvite
	{
		[Key]
		public int InviteID { get; set; }

		[Required]
		public int RFxID { get; set; }
		[ForeignKey("RFxID")]
		public virtual RFxEvent RFxEvent { get; set; }

		public int SupplierId { get; set; }
		[ForeignKey(nameof(SupplierId))]  
		public virtual Supplier Supplier { get; set; }

		

		public DateTime InvitedDate { get; set; } = DateTime.Now;

		[StringLength(20)]
		public InviteStatus Status { get; set; } 
	}
}
