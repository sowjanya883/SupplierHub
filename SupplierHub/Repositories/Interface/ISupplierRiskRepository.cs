using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface ISupplierRiskRepository
	{
		Task<SupplierRisk> CreateAsync(SupplierRisk entity, CancellationToken ct);
		Task<SupplierRisk?> GetByIdAsync(long riskId, CancellationToken ct);
		Task<IEnumerable<SupplierRisk>> GetAllAsync(CancellationToken ct);
		Task<SupplierRisk> UpdateAsync(SupplierRisk entity, CancellationToken ct);
		Task<bool> DeleteAsync(SupplierRiskDeleteDto dto, CancellationToken ct);
	}
}