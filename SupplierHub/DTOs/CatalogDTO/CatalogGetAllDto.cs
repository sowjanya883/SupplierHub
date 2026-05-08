public class CatalogGetAllDto{
	public long CatalogID { get; set; }
	public long SupplierID { get; set; }  
	public string CatalogName { get; set; }
	public DateTime? ValidFrom { get; set; } 
	public DateTime? ValidTo { get; set; } 
	public string Status { get; set; }
}
 