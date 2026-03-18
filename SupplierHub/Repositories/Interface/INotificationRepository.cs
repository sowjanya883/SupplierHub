using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface INotificationRepository
	{
		Task<List<Notification>> GetAllAsync(
			long? userId = null,
			string? status = null,
			string? category = null,
			bool includeDeleted = false,
			CancellationToken ct = default);

		Task<Notification?> GetByIdAsync(long id, bool includeDeleted = false, CancellationToken ct = default);

		Task AddAsync(Notification entity, CancellationToken ct = default);

		Task UpdateAsync(Notification entity, CancellationToken ct = default);

		Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default);

		Task<bool> RestoreAsync(long id, CancellationToken ct = default);

		Task<int> SaveChangesAsync(CancellationToken ct = default);

		Task<bool> ExistsAsync(long id, bool includeDeleted = false, CancellationToken ct = default);
	}
}
