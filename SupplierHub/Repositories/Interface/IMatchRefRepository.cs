using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IMatchRefRepository
	{
		Task<IEnumerable<MatchRef>> GetByInvoiceIdAsync(long invoiceId);
		Task<MatchRef?> GetByIdAsync(long id);
		Task AddAsync(MatchRef matchRef);
		void Update(MatchRef matchRef);
		Task SaveChangesAsync();
	}
}