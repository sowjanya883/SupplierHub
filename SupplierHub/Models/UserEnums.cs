namespace SupplierHub.Models.IAM
{
	public enum UserStatus
	{
		Active = 1,
		Locked = 2,
		Disabled = 3
	}

	public enum UserRole
	{
		Buyer = 1,
		CategoryManager = 2,
		Supplier = 3,
		Receiving = 4,
		AccountsPayable = 5,
		Admin = 6
	}
}
