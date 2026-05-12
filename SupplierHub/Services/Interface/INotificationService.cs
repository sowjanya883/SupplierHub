using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Models;

namespace SupplierHub.Services.Interface
{
	public interface INotificationService
	{
		Task<IEnumerable<Notification>> GetAllAsync(
			long? userId = null,
			string? status = null,
			string? category = null,
			bool includeDeleted = false,
			CancellationToken ct = default);

		Task<Notification?> GetByIdAsync(long id,
			bool includeDeleted = false,
			CancellationToken ct = default);

		Task<Notification> CreateAsync(Notification model,
			CancellationToken ct = default);

		Task<Notification?> UpdateAsync(long id, Notification model,
			CancellationToken ct = default);

		Task<bool> SoftDeleteAsync(long id,
			CancellationToken ct = default);

		Task<bool> RestoreAsync(long id,
			CancellationToken ct = default);

		Task<bool> DeleteNotificationAsync(long id);
		Task<bool> RestoreNotificationAsync(long id);

		// ── NEW: Fire-and-forget helpers ────────────────
		/// <summary>Send a notification to a single specific user.</summary>
		Task SendAsync(long userId, string message,
			string category, long? refEntityID = null);

		/// <summary>Send a notification to every active user with the given role name.</summary>
		Task SendToRoleAsync(string roleName, string message,
			string category, long? refEntityID = null);

		/// <summary>Mark all unread notifications for a user as Read.</summary>
		Task MarkAllReadAsync(long userId, CancellationToken ct = default);
	}
}