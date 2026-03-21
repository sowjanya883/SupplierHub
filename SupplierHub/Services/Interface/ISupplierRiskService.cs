using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.SupplierRiskDTO;

namespace SupplierHub.Services.Interface
{
	public interface ISupplierRiskService
	{
		Task<SupplierRiskGetByIdDto> CreateAsync(SupplierRiskCreateDto dto, CancellationToken ct);
		Task<SupplierRiskGetByIdDto?> GetByIdAsync(long riskId, CancellationToken ct);
		Task<IEnumerable<SupplierRiskGetAllDto>> GetAllAsync(CancellationToken ct);
		Task<SupplierRiskGetByIdDto?> UpdateAsync(SupplierRiskUpdateDto dto, CancellationToken ct);
		Task<bool> DeleteAsync(SupplierRiskDeleteDto dto, CancellationToken ct);
	}
}