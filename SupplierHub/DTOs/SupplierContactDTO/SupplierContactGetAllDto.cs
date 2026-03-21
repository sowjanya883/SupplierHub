using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierContactDTO
{
	public class SupplierContactGetAllDto
	{
		public long ContactID { get; set; }


		public required string SupplierName { get; set; }


		public string? Email { get; set; }


		public required string Status { get; set; }
	}
}