using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.ComplianceDocDTO;

namespace SupplierHub.Services.Interface
{
	public interface IComplianceDocService
	{
		Task<ComplianceDocGetByIdDto> CreateAsync(ComplianceDocCreateDto dto, CancellationToken ct);
		Task<ComplianceDocGetByIdDto?> GetByIdAsync(long docId, CancellationToken ct);
		Task<IEnumerable<ComplianceDocGetAllDto>> GetAllAsync(CancellationToken ct);
		Task<ComplianceDocGetByIdDto?> UpdateAsync(ComplianceDocUpdateDto dto, CancellationToken ct);
		Task<bool> DeleteAsync(ComplianceDocDeleteDto dto, CancellationToken ct);
	}
}