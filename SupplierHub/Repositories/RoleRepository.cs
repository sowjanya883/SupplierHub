using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class RoleRepository : IRoleRepository
	{
		private readonly AppDbContext _db;

		public RoleRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<List<Role>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
		{
			var query = _db.Roles.AsQueryable();
			if (!includeDeleted)
				query = query.Where(r => !r.IsDeleted);

			return await query.OrderByDescending(r => r.CreatedOn).ToListAsync(ct);
		}

		public async Task<Role?> GetByIdAsync(long id, bool includeDeleted = false, CancellationToken ct = default)
		{
			var entity = await _db.Roles.FirstOrDefaultAsync(r => r.RoleID == id, ct);
			if (entity == null) return null;
			if (!includeDeleted && entity.IsDeleted) return null;
			return entity;
		}

		public async Task AddAsync(Role entity, CancellationToken ct = default)
		{
			await _db.Roles.AddAsync(entity, ct);
		}

		public Task UpdateAsync(Role entity, CancellationToken ct = default)
		{
			_db.Roles.Update(entity);
			return Task.CompletedTask;
		}

		public async Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default)
		{
			var entity = await _db.Roles.FirstOrDefaultAsync(r => r.RoleID == id, ct);
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
			var entity = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.RoleID == id, ct);
			if (entity == null) return false;
			if (!includeDeleted && entity.IsDeleted) return false;
			return true;
		}
	}
}
