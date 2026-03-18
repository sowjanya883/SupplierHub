using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
    public interface IUserRoleRepository
    {
        Task<List<UserRole>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<List<UserRole>> GetByUserAsync(long userId, bool includeDeleted = false, CancellationToken ct = default);
        Task<UserRole?> GetByIdsAsync(long userId, long roleId, bool includeDeleted = false, CancellationToken ct = default);
        Task AddAsync(UserRole entity, CancellationToken ct = default);
        Task UpdateAsync(UserRole entity, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(long userId, long roleId, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<bool> ExistsAsync(long userId, long roleId, bool includeDeleted = false, CancellationToken ct = default);
    }
}

