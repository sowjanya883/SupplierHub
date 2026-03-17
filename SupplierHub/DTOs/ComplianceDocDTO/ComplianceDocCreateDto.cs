using System;
using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.ComplianceDocDTO
{
	public class ComplianceDocCreateDto
	{
		[Required]
		public long SupplierID { get; init; }

		[Required, MaxLength(50)]
		public required string DocType { get; init; }

		[MaxLength(500)]
		public string? FileUri { get; init; }

		public DateTime? IssueDate { get; init; }
		public DateTime? ExpiryDate { get; init; }
	}
}