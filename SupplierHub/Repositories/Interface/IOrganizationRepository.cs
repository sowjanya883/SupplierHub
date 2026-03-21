using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;
using SupplierHub.DTOs.OrganizationDTO;

namespace SupplierHub.Repositories.Interface
{
	public interface IOrganizationRepository
	{
		Task<Organization> CreateAsync(Organization entity, CancellationToken ct);
		Task<Organization?> GetByIdAsync(long orgId, CancellationToken ct);
		Task<IEnumerable<Organization>> GetAllAsync(CancellationToken ct);
		Task<Organization> UpdateAsync(Organization entity, CancellationToken ct);
		Task<bool> DeleteAsync(OrganizationDeleteDto dto, CancellationToken ct);
	}
}