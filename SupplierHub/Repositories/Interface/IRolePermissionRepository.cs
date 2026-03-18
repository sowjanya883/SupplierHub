using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
    public interface IRolePermissionRepository
    {
        Task<List<RolePermission>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<List<RolePermission>> GetByRoleAsync(long roleId, bool includeDeleted = false, CancellationToken ct = default);
        Task<RolePermission?> GetByIdsAsync(long roleId, long permissionId, bool includeDeleted = false, CancellationToken ct = default);
        Task AddAsync(RolePermission entity, CancellationToken ct = default);
        Task UpdateAsync(RolePermission entity, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(long roleId, long permissionId, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<bool> ExistsAsync(long roleId, long permissionId, bool includeDeleted = false, CancellationToken ct = default);
    }
}

