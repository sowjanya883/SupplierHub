using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IPoLineRepository
	{
		Task<IEnumerable<PoLine>> GetAllByPoIdAsync(long poId);
		Task<PoLine?> GetByIdAsync(long id);
		Task AddAsync(PoLine line);
		void Update(PoLine line);
		Task SaveChangesAsync();
	}
}