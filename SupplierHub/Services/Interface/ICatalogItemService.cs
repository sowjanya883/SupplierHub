using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.CatalogItemDTO;

namespace SupplierHub.Services.Interface
{
	public interface ICatalogItemService
	{
		Task<CatalogItemGetByIdDto> CreateAsync(CatalogItemCreateDto dto, CancellationToken ct);
		Task<CatalogItemGetByIdDto?> GetByIdAsync(long catItemId, CancellationToken ct);
		Task<IEnumerable<CatalogItemGetAllDto>> GetAllAsync(CancellationToken ct);
		Task<CatalogItemGetByIdDto?> UpdateAsync(CatalogItemUpdateDto dto, CancellationToken ct);
		Task<bool> DeleteAsync(CatalogItemDeleteDto dto, CancellationToken ct);
	}
}