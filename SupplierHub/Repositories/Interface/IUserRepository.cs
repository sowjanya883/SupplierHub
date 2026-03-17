using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<User?> GetByIdAsync(long id, bool includeDeleted = false, CancellationToken ct = default);
        Task AddAsync(User entity, CancellationToken ct = default);
        Task UpdateAsync(User entity, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<bool> ExistsAsync(long id, bool includeDeleted = false, CancellationToken ct = default);
    }
}
