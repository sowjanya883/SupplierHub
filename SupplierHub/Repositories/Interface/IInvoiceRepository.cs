using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IInvoiceRepository
	{
		Task<IEnumerable<Invoice>> GetAllAsync();
		Task<Invoice?> GetByIdAsync(long id);
		Task<IEnumerable<Invoice>> GetByPoIdAsync(long poId);
		Task AddAsync(Invoice invoice);
		void Update(Invoice invoice);
		Task SaveChangesAsync();
	}
}