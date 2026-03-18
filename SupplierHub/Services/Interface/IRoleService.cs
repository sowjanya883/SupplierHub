using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.RoleDTO;

namespace SupplierHub.Services.Interface
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<RoleDto?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken ct = default);
        Task<RoleDto?> UpdateAsync(long id, UpdateRoleDto dto, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default);
        Task<bool> ExistsAsync(long id, CancellationToken ct = default);
    }
}
