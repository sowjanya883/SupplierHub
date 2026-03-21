using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface ICatalogRepository
	{
		Task<Catalog> CreateAsync(Catalog entity, CancellationToken ct);
		Task<Catalog?> GetByIdAsync(long catalogId, CancellationToken ct);
		Task<IEnumerable<Catalog>> GetAllAsync(CancellationToken ct);
		Task<Catalog> UpdateAsync(Catalog entity, CancellationToken ct);
		Task<bool> DeleteAsync(long catalogId, CancellationToken ct);
	}
}