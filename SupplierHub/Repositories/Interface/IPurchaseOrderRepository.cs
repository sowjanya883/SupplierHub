using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IPurchaseOrderRepository
	{
		Task<PurchaseOrder?> GetByIdAsync(long id);
		Task<IEnumerable<PurchaseOrder>> GetAllActiveAsync();
		Task AddAsync(PurchaseOrder order);
		void Update(PurchaseOrder order);
		Task SaveChangesAsync();
	}
}
