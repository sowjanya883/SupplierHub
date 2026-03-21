using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.OrganizationDTO;

namespace SupplierHub.Services.Interface
{
	public interface IOrganizationService
	{
		Task<OrganizationGetByIdDto> CreateAsync(OrganizationCreateDto dto, CancellationToken ct);
		Task<OrganizationGetByIdDto?> GetByIdAsync(long orgId, CancellationToken ct);
		Task<IEnumerable<OrganizationGetAllDto>> GetAllAsync(CancellationToken ct);
		Task<OrganizationGetByIdDto?> UpdateAsync(OrganizationUpdateDto dto, CancellationToken ct);
		Task<bool> DeleteAsync(OrganizationDeleteDto dto, CancellationToken ct);
	}
}