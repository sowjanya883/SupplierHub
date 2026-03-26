using System;

namespace SupplierHub.Models
{
	public class UserRole
	{
		// ✅ Composite key columns
		public long UserID { get; set; }
		public long RoleID { get; set; }
		public string Status { get; set; } = null!;
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
		public bool IsDeleted { get; set; }

		// ✅ Navigation properties (REQUIRED)
		public User User { get; set; } = null!;
		public Role Role { get; set; } = null!;
	}
}