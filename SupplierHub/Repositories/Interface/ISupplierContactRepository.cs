using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;
using SupplierHub.DTOs.SupplierContactDTO;

namespace SupplierHub.Repositories.Interface
{
	public interface ISupplierContactRepository
	{
		Task<SupplierContact> CreateAsync(SupplierContact entity, CancellationToken ct);
		Task<SupplierContact?> GetByIdAsync(long contactId, CancellationToken ct);
		Task<IEnumerable<SupplierContact>> GetAllAsync(CancellationToken ct);
		Task<SupplierContact> UpdateAsync(SupplierContact entity, CancellationToken ct);
		Task<bool> DeleteAsync(SupplierContactDeleteDto dto, CancellationToken ct);
	}
}