using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.SupplierDTO;

namespace SupplierHub.Services.Interface
{
    public interface ISupplierService
    {
        Task<GetSupplierByIdDto?> CreateAsync(SupplierCreateDto dto, CancellationToken ct);
        Task<IEnumerable<GetAllSupplierDto>> GetAllAsync(CancellationToken ct);
        Task<GetSupplierByIdDto?> GetByIdAsync(long supplierId, CancellationToken ct);
        Task<GetSupplierByIdDto?> UpdateAsync(UpdateSupplierDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(SupplierDeleteDto dto, CancellationToken ct);
	

	// CancellationToken is used to stop an async operation early when the client cancels the request, saving server and database resources.
	}
}