namespace SupplierHub.Models
{
	public class UserRole
	{
		public long UserID { get; set; }
		public User User { get; set; } = null!;

		public long RoleID { get; set; }
		public string Status { get; set; } = null!;
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
		public bool IsDeleted { get; set; }

		public Role Role { get; set; } = null!;
	}
}