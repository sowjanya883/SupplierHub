using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class NotificationService : INotificationService
	{
		private readonly INotificationRepository _repo;
		private readonly IMapper _mapper;
		private readonly AppDbContext _db;

		public NotificationService(
			INotificationRepository repo,
			IMapper mapper,
			AppDbContext db)
		{
			_repo = repo;
			_mapper = mapper;
			_db = db;
		}

		public async Task<IEnumerable<Notification>> GetAllAsync(
			long? userId = null,
			string? status = null,
			string? category = null,
			bool includeDeleted = false,
			CancellationToken ct = default)
		{
			var list = await _repo.GetAllAsync(userId, status, category, includeDeleted, ct);
			return list;
		}

		public Task<Notification?> GetByIdAsync(long id,
			bool includeDeleted = false,
			CancellationToken ct = default)
			=> _repo.GetByIdAsync(id, includeDeleted, ct);

		public async Task<Notification> CreateAsync(
			Notification model,
			CancellationToken ct = default)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));
			var now = DateTime.UtcNow;
			model.NotificationID = 0;
			if (model.CreatedDate == default) model.CreatedDate = now;
			model.CreatedOn = now;
			model.UpdatedOn = now;
			model.IsDeleted = false;
			await _repo.AddAsync(model, ct);
			await _repo.SaveChangesAsync(ct);
			return model;
		}

		public async Task<Notification?> UpdateAsync(
			long id, Notification model,
			CancellationToken ct = default)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));
			var existing = await _repo.GetByIdAsync(id, includeDeleted: true, ct);
			if (existing == null || existing.IsDeleted) return null;
			_mapper.Map(model, existing);
			existing.NotificationID = id;
			existing.UpdatedOn = DateTime.UtcNow;
			await _repo.UpdateAsync(existing, ct);
			await _repo.SaveChangesAsync(ct);
			return existing;
		}

		public async Task<bool> SoftDeleteAsync(long id,
			CancellationToken ct = default)
		{
			var ok = await _repo.SoftDeleteAsync(id, ct);
			if (!ok) return false;
			await _repo.SaveChangesAsync(ct);
			return true;
		}

		public async Task<bool> RestoreAsync(long id,
			CancellationToken ct = default)
		{
			var ok = await _repo.RestoreAsync(id, ct);
			if (!ok) return false;
			await _repo.SaveChangesAsync(ct);
			return true;
		}

		public Task<bool> DeleteNotificationAsync(long id)
			=> SoftDeleteAsync(id, CancellationToken.None);

		public Task<bool> RestoreNotificationAsync(long id)
			=> RestoreAsync(id, CancellationToken.None);

		// ── NEW helpers ────────────────────────────────

		public async Task SendAsync(
			long userId, string message,
			string category, long? refEntityID = null)
		{
			// Silently ignore if user doesn't exist
			var exists = await _db.Users
				.AnyAsync(u => u.UserID == userId && !u.IsDeleted);
			if (!exists) return;

			var now = DateTime.UtcNow;
			var notif = new Notification
			{
				UserID = userId,
				Message = message,
				Category = category,
				RefEntityID = refEntityID,
				Status = "Unread",
				CreatedDate = now,
				CreatedOn = now,
				UpdatedOn = now,
				IsDeleted = false,
			};
			_db.Notifications.Add(notif);
			await _db.SaveChangesAsync();
		}

		public async Task SendToRoleAsync(
			string roleName, string message,
			string category, long? refEntityID = null)
		{
			// Find all active users with the specified role
			var userIds = await (
				from ur in _db.UserRoles
				join r in _db.Roles on ur.RoleID equals r.RoleID
				where !ur.IsDeleted
				   && !r.IsDeleted
				   && r.RoleName == roleName
				select ur.UserID
			).Distinct().ToListAsync();

			if (!userIds.Any()) return;

			var now = DateTime.UtcNow;
			var notifications = userIds.Select(uid => new Notification
			{
				UserID = uid,
				Message = message,
				Category = category,
				RefEntityID = refEntityID,
				Status = "Unread",
				CreatedDate = now,
				CreatedOn = now,
				UpdatedOn = now,
				IsDeleted = false,
			}).ToList();

			_db.Notifications.AddRange(notifications);
			await _db.SaveChangesAsync();
		}

		public async Task MarkAllReadAsync(
			long userId,
			CancellationToken ct = default)
		{
			var unread = await _db.Notifications
				.Where(n => n.UserID == userId
						 && n.Status == "Unread"
						 && !n.IsDeleted)
				.ToListAsync(ct);

			if (!unread.Any()) return;

			var now = DateTime.UtcNow;
			foreach (var n in unread)
			{
				n.Status = "Read";
				n.UpdatedOn = now;
			}
			await _db.SaveChangesAsync(ct);
		}
	}
}