using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;
using SupplierHub.DTOs.ContractDTO;

namespace SupplierHub.Repositories.Interface
{
	public interface IContractRepository
	{
		Task<Contract> CreateAsync(Contract entity, CancellationToken ct);
		Task<Contract?> GetByIdAsync(long contractId, CancellationToken ct);
		Task<IEnumerable<Contract>> GetAllAsync(CancellationToken ct);
		Task<Contract> UpdateAsync(Contract entity, CancellationToken ct);
		Task<bool> DeleteAsync(ContractDeleteDto dto, CancellationToken ct);
	}
}
