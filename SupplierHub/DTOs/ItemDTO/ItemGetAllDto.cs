using System.ComponentModel.DataAnnotations;
namespace SupplierHub.DTOs.ItemDTO
{
	public class ItemGetAllDto	{
		public long ItemID { get; set; }
		public long CategoryID { get; set; }   
		public string Sku { get; set; }
		public string? Description { get; set; } 
		public string? Uom { get; set; }      
		public int? LeadTimeDays { get; set; }  
		public string Status { get; set; }
	}
}
 