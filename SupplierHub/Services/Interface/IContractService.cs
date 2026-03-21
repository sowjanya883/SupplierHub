using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.ContractDTO;

namespace SupplierHub.Services.Interface
{
	public interface IContractService
	{
		Task<ContractGetByIdDto> CreateAsync(ContractCreateDto dto, CancellationToken ct);
		Task<ContractGetByIdDto?> GetByIdAsync(long contractId, CancellationToken ct);
		Task<IEnumerable<ContractGetAllDto>> GetAllAsync(CancellationToken ct);
		Task<ContractGetByIdDto?> UpdateAsync(ContractUpdateDto dto, CancellationToken ct);
		Task<bool> DeleteAsync(ContractDeleteDto dto, CancellationToken ct);
	}
}
