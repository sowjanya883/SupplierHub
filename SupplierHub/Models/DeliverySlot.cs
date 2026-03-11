using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplierHub.Models
{
	[Table("DeliverySlot")]
	public class DeliverySlot
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long SlotID { get; set; }

		[Required]
		[ForeignKey(nameof(Site))]
		public long SiteID { get; set; }

		[Required]
		public DateTime SlotDate { get; set; }

		[Required]
		public TimeSpan StartTime { get; set; }

		[Required]
		public TimeSpan EndTime { get; set; }

		[Required]
		public int Capacity { get; set; }

		[Required]
		[StringLength(20)]
		public string Status { get; set; }

		// Navigation Properties
		public virtual Site Site { get; set; }
	}
}