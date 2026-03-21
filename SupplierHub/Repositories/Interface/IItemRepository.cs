using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;
using SupplierHub.DTOs.ItemDTO;

namespace SupplierHub.Repositories.Interface
{
	public interface IItemRepository
	{
		Task<Item> CreateAsync(Item entity, CancellationToken ct);
		Task<Item?> GetByIdAsync(long itemId, CancellationToken ct);
		Task<IEnumerable<Item>> GetAllAsync(CancellationToken ct);
		Task<Item> UpdateAsync(Item entity, CancellationToken ct);
		Task<bool> DeleteAsync(ItemDeleteDto dto, CancellationToken ct);
	}
}