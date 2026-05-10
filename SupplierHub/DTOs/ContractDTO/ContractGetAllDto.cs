public class ContractGetAllDto{
	public long ContractID { get; set; }
	public long SupplierID { get; set; }
	public long? ItemID { get; set; }      
	public string? Currency { get; set; }  
	public decimal? Rate { get; set; }
	public DateTime? ValidFrom { get; set; }
	public DateTime? ValidTo { get; set; }  
	public string Status { get; set; }
}
 