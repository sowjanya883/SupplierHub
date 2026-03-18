using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _db;

        public UserRoleRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<UserRole>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var query = _db.Set<UserRole>().AsQueryable();
            if (!includeDeleted)
                query = query.Where(ur => !ur.IsDeleted);

            return await query.OrderByDescending(ur => ur.CreatedOn).ToListAsync(ct);
        }

        public async Task<List<UserRole>> GetByUserAsync(long userId, bool includeDeleted = false, CancellationToken ct = default)
        {
            var query = _db.Set<UserRole>().Where(ur => ur.UserID == userId).AsQueryable();
            if (!includeDeleted)
                query = query.Where(ur => !ur.IsDeleted);

            return await query.OrderByDescending(ur => ur.CreatedOn).ToListAsync(ct);
        }

        public async Task<UserRole?> GetByIdsAsync(long userId, long roleId, bool includeDeleted = false, CancellationToken ct = default)
        {
            var entity = await _db.Set<UserRole>().FirstOrDefaultAsync(ur => ur.UserID == userId && ur.RoleID == roleId, ct);
            if (entity == null) return null;
            if (!includeDeleted && entity.IsDeleted) return null;
            return entity;
        }

        public async Task AddAsync(UserRole entity, CancellationToken ct = default)
        {
            await _db.Set<UserRole>().AddAsync(entity, ct);
        }

        public Task UpdateAsync(UserRole entity, CancellationToken ct = default)
        {
            _db.Set<UserRole>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> SoftDeleteAsync(long userId, long roleId, CancellationToken ct = default)
        {
            var entity = await _db.Set<UserRole>().FirstOrDefaultAsync(ur => ur.UserID == userId && ur.RoleID == roleId, ct);
            if (entity == null || entity.IsDeleted) return false;
            entity.IsDeleted = true;
            entity.UpdatedOn = System.DateTime.UtcNow;
            return true;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(long userId, long roleId, bool includeDeleted = false, CancellationToken ct = default)
        {
            var entity = await _db.Set<UserRole>().AsNoTracking().FirstOrDefaultAsync(ur => ur.UserID == userId && ur.RoleID == roleId, ct);
            if (entity == null) return false;
            if (!includeDeleted && entity.IsDeleted) return false;
            return true;
        }
    }
}

