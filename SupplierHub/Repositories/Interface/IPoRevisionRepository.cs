using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IPoRevisionRepository
	{
		Task<IEnumerable<PoRevision>> GetAllByPoIdAsync(long poId);
		Task<PoRevision?> GetByIdAsync(long id);
		Task<int> GetMaxRevisionNoAsync(long poId);
		Task AddAsync(PoRevision revision);
		Task SaveChangesAsync();
	}
}