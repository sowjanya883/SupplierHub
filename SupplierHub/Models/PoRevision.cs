using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.Models
{
	public class PoRevision
	{
		[Key]
		public long PorevID { get; set; }

		[Required]
		public long PoID { get; set; }

		[Required]
		public int RevisionNo { get; set; }

		public long? ChangedBy { get; set; }

		public string? ChangelogJson { get; set; }

		public DateTime? ChangeDate { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		[Required, MaxLength(30)]
		public required string Status { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; }

		[Required]
		public DateTime UpdatedOn { get; set; }
	}
}