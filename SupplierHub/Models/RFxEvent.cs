using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplierHub.Constants.Enum;
using SupplierHub.Models;
using SupplierHub.Models.IAM;
namespace SupplierHub.Models
{
	public class RFxEvent
	{
		[Key]
		public int RFxID { get; set; }

		[Required]
		public RFxType Type { get; set; } 

		[Required]
		[StringLength(200)]
		public string? Title { get; set; }
		
		public int CategoryID { get; set; }
		[ForeignKey(nameof(CategoryID))]

		public Category Category { get; set; }


		public int CreatedBy { get; set; }
		[ForeignKey(nameof(CreatedBy))]

		public virtual User? User { get; set; }


		[Required]
		public DateTime OpenDate { get; set; }

		[Required]
		public DateTime CloseDate { get; set; }

		[Required]
		[StringLength(20)]
		public RFxStatus Status { get; set; }

		public Award Award { get; set; }
		public virtual ICollection<RFxLine> RFxLines { get; set; }
		public virtual ICollection<RFxInvite> RFxInvites { get; set; }
		public virtual ICollection<Bid> Bids { get; set; }


	}
}
