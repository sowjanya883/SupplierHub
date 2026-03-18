using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
    public interface IAuditLogRepository
    {
        Task<List<AuditLog>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<AuditLog?> GetByIdAsync(long id, bool includeDeleted = false, CancellationToken ct = default);
        Task AddAsync(AuditLog entity, CancellationToken ct = default);
        Task UpdateAsync(AuditLog entity, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<bool> ExistsAsync(long id, bool includeDeleted = false, CancellationToken ct = default);
    }
}

