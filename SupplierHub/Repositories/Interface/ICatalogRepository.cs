using System.Collections.Generic;
using System.Threading.Tasks;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface ICatalogRepository
	{
		Task<Catalog> CreateAsync(Catalog entity);
		Task<Catalog?> GetByIdAsync(long catalogId);
		Task<IEnumerable<Catalog>> GetAllAsync();
		Task<Catalog> UpdateAsync(Catalog entity);
		Task<bool> DeleteAsync(long catalogId);
	}
}