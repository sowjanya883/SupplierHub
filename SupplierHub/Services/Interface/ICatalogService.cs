using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.CatalogDTO;

namespace SupplierHub.Services.Interface
{
	public interface ICatalogService
	{
		Task<CatalogCreateDto> CreateAsync(
			CatalogCreateDto dto,
			CancellationToken ct = default);

		Task<IEnumerable<object>> GetAllAsync(
			CancellationToken ct = default);

		Task<object?> GetByIdAsync(
			long catalogId,
			CancellationToken ct = default);

		Task<object?> UpdateAsync(
			CatalogUpdateDto dto,
			CancellationToken ct = default);

		Task<bool> DeleteAsync(
			CatalogDeleteDto dto,
			CancellationToken ct = default);
	}
}