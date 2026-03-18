using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _db;

        public AuditLogRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<AuditLog>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var query = _db.Set<AuditLog>().AsQueryable();
            if (!includeDeleted)
                query = query.Where(a => !a.IsDeleted);

            return await query.OrderByDescending(a => a.Timestamp).ToListAsync(ct);
        }

        public async Task<AuditLog?> GetByIdAsync(long id, bool includeDeleted = false, CancellationToken ct = default)
        {
            var entity = await _db.Set<AuditLog>().FirstOrDefaultAsync(a => a.AuditID == id, ct);
            if (entity == null) return null;
            if (!includeDeleted && entity.IsDeleted) return null;
            return entity;
        }

        public async Task AddAsync(AuditLog entity, CancellationToken ct = default)
        {
            await _db.Set<AuditLog>().AddAsync(entity, ct);
        }

        public Task UpdateAsync(AuditLog entity, CancellationToken ct = default)
        {
            _db.Set<AuditLog>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default)
        {
            var entity = await _db.Set<AuditLog>().FirstOrDefaultAsync(a => a.AuditID == id, ct);
            if (entity == null || entity.IsDeleted) return false;
            entity.IsDeleted = true;
            entity.UpdatedOn = System.DateTime.UtcNow;
            return true;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(long id, bool includeDeleted = false, CancellationToken ct = default)
        {
            var entity = await _db.Set<AuditLog>().AsNoTracking().FirstOrDefaultAsync(a => a.AuditID == id, ct);
            if (entity == null) return false;
            if (!includeDeleted && entity.IsDeleted) return false;
            return true;
        }
    }
}

