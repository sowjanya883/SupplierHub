using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class DeliverySlot
	{
		[Key]
		public long SlotID { get; set; }

		[Required]
		public long SiteID { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required]
		public TimeSpan StartTime { get; set; }

		[Required]
		public TimeSpan EndTime { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		public int? Capacity { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }


		[Required]

		public DateTime UpdatedOn { get; set; }

	}
}