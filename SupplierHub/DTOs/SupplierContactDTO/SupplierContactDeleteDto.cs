using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierContactDTO
{
	public class SupplierContactDeleteDto
	{
		public long? ContactID { get; set; }


		public long? SupplierID { get; set; }


		public string? Email { get; set; }



		public string? Phone { get; set; }
	}
}