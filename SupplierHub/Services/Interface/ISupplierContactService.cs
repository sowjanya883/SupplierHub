using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.SupplierContactDTO;

namespace SupplierHub.Services.Interface
{
	public interface ISupplierContactService
	{
		Task<SupplierContactGetByIdDto> CreateAsync(SupplierContactCreateDto dto, CancellationToken ct);
		Task<SupplierContactGetByIdDto?> GetByIdAsync(long contactId, CancellationToken ct);
		Task<IEnumerable<SupplierContactGetAllDto>> GetAllAsync(CancellationToken ct);
		Task<SupplierContactGetByIdDto?> UpdateAsync(SupplierContactUpdateDto dto, CancellationToken ct);
		Task<bool> DeleteAsync(SupplierContactDeleteDto dto, CancellationToken ct);
	}
}