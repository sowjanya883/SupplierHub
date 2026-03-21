using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.CategoryDTO;

namespace SupplierHub.Services.Interface
{
	public interface ICategoryService
	{
		Task<CategoryGetByIdDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct);
		Task<CategoryGetByIdDto?> GetByIdAsync(long categoryId, CancellationToken ct);
		Task<IEnumerable<CategoryGetAllDto>> GetAllAsync(CancellationToken ct);
		Task<CategoryGetByIdDto?> UpdateAsync(CategoryUpdateDto dto, CancellationToken ct);
		Task<bool> DeleteAsync(CategoryDeleteDto dto, CancellationToken ct);
	}
}