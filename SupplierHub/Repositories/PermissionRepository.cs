using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _db;

        public PermissionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Permission>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var query = _db.Set<Permission>().AsQueryable();
            if (!includeDeleted)
                query = query.Where(p => !p.IsDeleted);

            return await query.OrderByDescending(p => p.CreatedOn).ToListAsync(ct);
        }

        public async Task<Permission?> GetByIdAsync(long id, bool includeDeleted = false, CancellationToken ct = default)
        {
            var entity = await _db.Set<Permission>().FirstOrDefaultAsync(p => p.PermissionID == id, ct);
            if (entity == null) return null;
            if (!includeDeleted && entity.IsDeleted) return null;
            return entity;
        }

        public async Task AddAsync(Permission entity, CancellationToken ct = default)
        {
            await _db.Set<Permission>().AddAsync(entity, ct);
        }

        public Task UpdateAsync(Permission entity, CancellationToken ct = default)
        {
            _db.Set<Permission>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default)
        {
            var entity = await _db.Set<Permission>().FirstOrDefaultAsync(p => p.PermissionID == id, ct);
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
            var entity = await _db.Set<Permission>().AsNoTracking().FirstOrDefaultAsync(p => p.PermissionID == id, ct);
            if (entity == null) return false;
            if (!includeDeleted && entity.IsDeleted) return false;
            return true;
        }
    }
}

