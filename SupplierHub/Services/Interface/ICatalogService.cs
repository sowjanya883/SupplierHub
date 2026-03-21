using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.CatalogDTO;

namespace SupplierHub.Services.Interface
{
	public interface ICatalogService
	{
		Task<CatalogGetByIdDto> CreateAsync(CatalogCreateDto dto, CancellationToken ct);
		Task<CatalogGetByIdDto?> GetByIdAsync(long itemId, CancellationToken ct);
		Task<IEnumerable<CatalogGetAllDto>> GetAllAsync(CancellationToken ct);
		Task<CatalogGetByIdDto?> UpdateAsync(CatalogUpdateDto dto, CancellationToken ct);
		Task<bool> DeleteAsync(CatalogDeleteDto dto, CancellationToken ct);
	}
}