using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;
using SupplierHub.DTOs.CatalogItemDTO;

namespace SupplierHub.Repositories.Interface
{
	public interface ICatalogItemRepository
	{
		Task<CatalogItem> CreateAsync(CatalogItem entity, CancellationToken ct);
		Task<CatalogItem?> GetByIdAsync(long catItemId, CancellationToken ct);
		Task<IEnumerable<CatalogItem>> GetAllAsync(CancellationToken ct);
		Task<CatalogItem> UpdateAsync(CatalogItem entity, CancellationToken ct);
		Task<bool> DeleteAsync(CatalogItemDeleteDto dto, CancellationToken ct);
	}
}