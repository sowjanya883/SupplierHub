using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;

namespace SupplierHub.Services.Interface
{
	public interface INotificationService
	{
		/// <summary>
		/// Get all notifications (by default excludes soft-deleted).
		/// </summary>
		Task<IEnumerable<Notification>> GetAllAsync(
			long? userId = null,
			string? status = null,
			string? category = null,
			bool includeDeleted = false,
			CancellationToken ct = default);

		/// <summary>
		/// Get a single notification by id (returns null if not found or soft-deleted unless includeDeleted=true).
		/// </summary>
		Task<Notification?> GetByIdAsync(long id, bool includeDeleted = false, CancellationToken ct = default);

		/// <summary>
		/// Create a new notification. Server sets timestamps and defaults.
		/// </summary>
		Task<Notification> CreateAsync(Notification model, CancellationToken ct = default);

		/// <summary>
		/// Update an existing notification by id. Returns null if not found or soft-deleted.
		/// </summary>
		Task<Notification?> UpdateAsync(long id, Notification model, CancellationToken ct = default);

		/// <summary>
		/// Soft delete a notification by id. Returns false if not found or already deleted.
		/// </summary>
		Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default);

		/// <summary>
		/// Restore a soft-deleted notification by id. Returns false if not found or not deleted.
		/// </summary>
		Task<bool> RestoreAsync(long id, CancellationToken ct = default);
		
 Task<bool> DeleteNotificationAsync(long id);    // soft delete
        Task<bool> RestoreNotificationAsync(long id);

	}
}
