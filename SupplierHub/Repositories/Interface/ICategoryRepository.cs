using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;
using SupplierHub.DTOs.CategoryDTO;

namespace SupplierHub.Repositories.Interface
{
	public interface ICategoryRepository
	{
		Task<Category> CreateAsync(Category entity, CancellationToken ct);
		Task<Category?> GetByIdAsync(long categoryId, CancellationToken ct);
		Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct);
		Task<Category> UpdateAsync(Category entity, CancellationToken ct);
		Task<bool> DeleteAsync(CategoryDeleteDto dto, CancellationToken ct);
	}
}