using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface ISuppliersRepository
	{
		Task<Supplier> CreateAsync(Supplier entity, CancellationToken ct);
		Task<Supplier?> GetByIdAsync(long supplierId, CancellationToken ct);
		Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken ct);
		Task<Supplier> UpdateAsync(Supplier entity, CancellationToken ct);
		Task<bool> DeleteAsync(SupplierDeleteDto dto, CancellationToken ct);
	}
}