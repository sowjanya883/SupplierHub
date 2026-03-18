using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.RolePermissionDTO;

namespace SupplierHub.Services.Interface
{
    public interface IRolePermissionService
    {
        Task<List<RolePermissionDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<List<RolePermissionDto>> GetByRoleAsync(long roleId, bool includeDeleted = false, CancellationToken ct = default);
        Task<RolePermissionDto?> GetByIdsAsync(long roleId, long permissionId, CancellationToken ct = default);
        Task<RolePermissionDto> CreateAsync(CreateRolePermissionDto dto, CancellationToken ct = default);
        Task<RolePermissionDto?> UpdateAsync(long roleId, long permissionId, UpdateRolePermissionDto dto, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(long roleId, long permissionId, CancellationToken ct = default);
        Task<bool> ExistsAsync(long roleId, long permissionId, CancellationToken ct = default);
    }
}

