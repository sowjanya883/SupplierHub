using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class NotificationRepository : INotificationRepository
	{
		private readonly AppDbContext _db;

		public NotificationRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<List<Notification>> GetAllAsync(
			long? userId = null,
			string? status = null,
			string? category = null,
			bool includeDeleted = false,
			CancellationToken ct = default)
		{
			var query = _db.Notifications
				.AsNoTracking()
				.AsQueryable();

			if (!includeDeleted)
				query = query.Where(n => !n.IsDeleted);

			if (userId.HasValue)
				query = query.Where(n => n.UserID == userId.Value);

			if (!string.IsNullOrWhiteSpace(status))
				query = query.Where(n => n.Status.ToLower() == status.ToLower());

			if (!string.IsNullOrWhiteSpace(category))
				query = query.Where(n => n.Category != null && n.Category.ToLower() == category.ToLower());

			// Newest first
			query = query.OrderByDescending(n => n.CreatedOn);

			return await query.ToListAsync(ct);
		}

		public async Task<Notification?> GetByIdAsync(long id, bool includeDeleted = false, CancellationToken ct = default)
		{
			var entity = await _db.Notifications
				.FirstOrDefaultAsync(n => n.NotificationID == id, ct);

			if (entity == null)
				return null;

			if (!includeDeleted && entity.IsDeleted)
				return null;

			return entity;
		}

		public async Task AddAsync(Notification entity, CancellationToken ct = default)
		{
			await _db.Notifications.AddAsync(entity, ct);
		}

		public Task UpdateAsync(Notification entity, CancellationToken ct = default)
		{
			_db.Notifications.Update(entity);
			return Task.CompletedTask;
		}

		public async Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default)
		{
			var entity = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationID == id, ct);
			if (entity == null || entity.IsDeleted)
				return false;

			entity.IsDeleted = true;
			entity.UpdatedOn = System.DateTime.UtcNow;

			return true; // caller should SaveChangesAsync()
		}

		public async Task<bool> RestoreAsync(long id, CancellationToken ct = default)
		{
			var entity = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationID == id, ct);
			if (entity == null || !entity.IsDeleted)
				return false;

			entity.IsDeleted = false;
			entity.UpdatedOn = System.DateTime.UtcNow;

			return true; // caller should SaveChangesAsync()
		}

		public Task<int> SaveChangesAsync(CancellationToken ct = default)
		{
			return _db.SaveChangesAsync(ct);
		}

		public async Task<bool> ExistsAsync(long id, bool includeDeleted = false, CancellationToken ct = default)
		{
			var entity = await _db.Notifications
				.AsNoTracking()
				.FirstOrDefaultAsync(n => n.NotificationID == id, ct);

			if (entity == null) return false;
			if (!includeDeleted && entity.IsDeleted) return false;

			return true;
		}
	}
}
