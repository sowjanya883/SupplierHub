using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.ItemDTO;

namespace SupplierHub.Services.Interface
{
	public interface IItemService
	{
		Task<ItemGetByIdDto> CreateAsync(ItemCreateDto dto, CancellationToken ct);
		Task<ItemGetByIdDto?> GetByIdAsync(long itemId, CancellationToken ct);
		Task<IEnumerable<ItemGetAllDto>> GetAllAsync(CancellationToken ct);
		Task<ItemGetByIdDto?> UpdateAsync(ItemUpdateDto dto, CancellationToken ct);
		Task<bool> DeleteAsync(ItemDeleteDto dto, CancellationToken ct);
	}
}