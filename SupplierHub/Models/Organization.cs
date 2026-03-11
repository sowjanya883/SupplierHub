using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Constants;

namespace SupplierHub.Models
{
	[Table("organization")]
	[Index(nameof(Status), Name = "idx_org_status")]
	[Index(nameof(UpdatedOn), Name = "idx_org_updatedon")]
	public class Organization
	{
		[Key]
		public long OrgId { get; set; }  

		[Required, MaxLength(200)]
		public string Name { get; set; } = default!;  

		// JSON column (MySQL) – configured via Fluent API
		public string? AddressJson { get; set; }     

		[MaxLength(50)]
		public string? TaxId { get; set; }            

		[Required]
		public OrganizationStatus Status { get; set; } = OrganizationStatus.Active; 

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow; 

		public DateTime? UpdatedOn { get; set; } 
	}
}