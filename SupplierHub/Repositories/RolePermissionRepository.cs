using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly AppDbContext _db;

        public RolePermissionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<RolePermission>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var query = _db.Set<RolePermission>().AsQueryable();
            if (!includeDeleted)
                query = query.Where(rp => !rp.IsDeleted);

            return await query.OrderByDescending(rp => rp.CreatedOn).ToListAsync(ct);
        }

        public async Task<List<RolePermission>> GetByRoleAsync(long roleId, bool includeDeleted = false, CancellationToken ct = default)
        {
            var query = _db.Set<RolePermission>().Where(rp => rp.RoleID == roleId).AsQueryable();
            if (!includeDeleted)
                query = query.Where(rp => !rp.IsDeleted);

            return await query.OrderByDescending(rp => rp.CreatedOn).ToListAsync(ct);
        }

        public async Task<RolePermission?> GetByIdsAsync(long roleId, long permissionId, bool includeDeleted = false, CancellationToken ct = default)
        {
            var entity = await _db.Set<RolePermission>().FirstOrDefaultAsync(rp => rp.RoleID == roleId && rp.PermissionID == permissionId, ct);
            if (entity == null) return null;
            if (!includeDeleted && entity.IsDeleted) return null;
            return entity;
        }

        public async Task AddAsync(RolePermission entity, CancellationToken ct = default)
        {
            await _db.Set<RolePermission>().AddAsync(entity, ct);
        }

        public Task UpdateAsync(RolePermission entity, CancellationToken ct = default)
        {
            _db.Set<RolePermission>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> SoftDeleteAsync(long roleId, long permissionId, CancellationToken ct = default)
        {
            var entity = await _db.Set<RolePermission>().FirstOrDefaultAsync(rp => rp.RoleID == roleId && rp.PermissionID == permissionId, ct);
            if (entity == null || entity.IsDeleted) return false;
            entity.IsDeleted = true;
            entity.UpdatedOn = System.DateTime.UtcNow;
            return true;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(long roleId, long permissionId, bool includeDeleted = false, CancellationToken ct = default)
        {
            var entity = await _db.Set<RolePermission>().AsNoTracking().FirstOrDefaultAsync(rp => rp.RoleID == roleId && rp.PermissionID == permissionId, ct);
            if (entity == null) return false;
            if (!includeDeleted && entity.IsDeleted) return false;
            return true;
        }
    }
}

