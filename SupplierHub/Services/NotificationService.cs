using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class NotificationService : INotificationService
	{
		private readonly INotificationRepository _repo;
		private readonly IMapper _mapper;

		public NotificationService(INotificationRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<IEnumerable<Notification>> GetAllAsync(
			long? userId = null,
			string? status = null,
			string? category = null,
			bool includeDeleted = false,
			CancellationToken ct = default)
		{
			var list = await _repo.GetAllAsync(userId, status, category, includeDeleted, ct);
			return list; // List<Notification> is an IEnumerable<Notification>
		}

		public Task<Notification?> GetByIdAsync(long id, bool includeDeleted = false, CancellationToken ct = default)
			=> _repo.GetByIdAsync(id, includeDeleted, ct);

		public async Task<Notification> CreateAsync(Notification model, CancellationToken ct = default)
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

		public async Task<Notification?> UpdateAsync(long id, Notification model, CancellationToken ct = default)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));

			var existing = await _repo.GetByIdAsync(id, includeDeleted: true, ct);
			if (existing == null || existing.IsDeleted) return null;

			// Use AutoMapper to apply fields safely (configure mapping to ignore immutables)
			_mapper.Map(model, existing);

			existing.NotificationID = id;
			// Keep immutables as-is (UserID, CreatedDate, CreatedOn)
			existing.UpdatedOn = DateTime.UtcNow;
			existing.IsDeleted = existing.IsDeleted; // do not change here

			await _repo.UpdateAsync(existing, ct);
			await _repo.SaveChangesAsync(ct);
			return existing;
		}

		public async Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default)
		{
			var ok = await _repo.SoftDeleteAsync(id, ct);
			if (!ok) return false;
			await _repo.SaveChangesAsync(ct);
			return true;
		}

		public async Task<bool> RestoreAsync(long id, CancellationToken ct = default)
		{
			var ok = await _repo.RestoreAsync(id, ct);
			if (!ok) return false;
			await _repo.SaveChangesAsync(ct);
			return true;
		}

		public Task<bool> DeleteNotificationAsync(long id) => SoftDeleteAsync(id, CancellationToken.None);
		public Task<bool> RestoreNotificationAsync(long id) => RestoreAsync(id, CancellationToken.None);
	}
}