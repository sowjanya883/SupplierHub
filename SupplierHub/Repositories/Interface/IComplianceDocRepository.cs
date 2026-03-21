using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IComplianceDocRepository
	{
		Task<ComplianceDoc> CreateAsync(ComplianceDoc entity, CancellationToken ct);
		Task<ComplianceDoc?> GetByIdAsync(long docId, CancellationToken ct);
		Task<IEnumerable<ComplianceDoc>> GetAllAsync(CancellationToken ct);
		Task<ComplianceDoc> UpdateAsync(ComplianceDoc entity, CancellationToken ct);
		Task<bool> DeleteAsync(ComplianceDocDeleteDto dto, CancellationToken ct);
	}
}