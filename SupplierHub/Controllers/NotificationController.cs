using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupplierHub.Models;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	// Simple DTO for PATCH — avoids required-field binding issues
	public class UpdateNotifStatusDto
	{
		public string Status { get; set; } = "Read";
	}

	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationService _service;
		private readonly ILogger<NotificationController> _logger;

		public NotificationController(
			INotificationService service,
			ILogger<NotificationController> logger)
		{
			_service = service;
			_logger = logger;
		}

		// GET api/notification
		[HttpGet]
		public async Task<IActionResult> GetAll(
			[FromQuery] string? status = null,
			[FromQuery] string? category = null,
			CancellationToken ct = default)
		{
			try
			{
				var userId = GetCurrentUserId();
				if (userId == null)
					return Unauthorized(new { message = "User identity not found." });

				var items = await _service.GetAllAsync(
					userId: userId,
					status: status,
					category: category,
					ct: ct);

				return Ok(new { data = items, count = items.Count() });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching notifications.");
				return StatusCode(500, new { message = "Error retrieving notifications." });
			}
		}

		// GET api/notification/{id}
		[HttpGet("{id:long}")]
		public async Task<IActionResult> GetById(
			long id, CancellationToken ct = default)
		{
			try
			{
				var entity = await _service.GetByIdAsync(id, ct: ct);
				if (entity == null)
					return NotFound(new { message = $"Notification {id} not found." });

				var userId = GetCurrentUserId();
				if (entity.UserID != userId && !User.IsInRole("Admin"))
					return Forbid();

				return Ok(entity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching notification {Id}", id);
				return StatusCode(500, new { message = "Error retrieving notification." });
			}
		}

		// PATCH api/notification/{id} — mark as Read / Dismissed
		[HttpPatch("{id:long}")]
		public async Task<IActionResult> UpdateStatus(
			long id,
			[FromBody] UpdateNotifStatusDto dto,
			CancellationToken ct = default)
		{
			if (dto == null)
				return BadRequest(new { message = "Request body cannot be empty." });

			try
			{
				var entity = await _service.GetByIdAsync(id, ct: ct);
				if (entity == null)
					return NotFound(new { message = $"Notification {id} not found." });

				var userId = GetCurrentUserId();
				if (entity.UserID != userId && !User.IsInRole("Admin"))
					return Forbid();

				// Only update Status — all other fields stay intact
				entity.Status = dto.Status;
				entity.UpdatedOn = DateTime.UtcNow;

				await _service.UpdateAsync(id, entity, ct);

				return Ok(new
				{
					message = "Notification status updated successfully.",
					data = entity
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error patching notification {Id}", id);
				return StatusCode(500, new { message = "An error occurred during the update." });
			}
		}

		// POST api/notification/mark-all-read
		[HttpPost("mark-all-read")]
		public async Task<IActionResult> MarkAllRead(CancellationToken ct = default)
		{
			try
			{
				var userId = GetCurrentUserId();
				if (userId == null)
					return Unauthorized();

				await _service.MarkAllReadAsync(userId.Value, ct);
				return Ok(new { message = "All notifications marked as read." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error marking all read.");
				return StatusCode(500, new { message = "Error marking notifications as read." });
			}
		}

		// DELETE api/notification/{id}
		[HttpDelete("{id:long}")]
		public async Task<IActionResult> Delete(
			long id, CancellationToken ct = default)
		{
			try
			{
				var entity = await _service.GetByIdAsync(id, ct: ct);
				if (entity == null)
					return NotFound(new { message = $"Notification {id} not found." });

				var userId = GetCurrentUserId();
				if (entity.UserID != userId && !User.IsInRole("Admin"))
					return Forbid();

				await _service.SoftDeleteAsync(id, ct);
				return Ok(new { message = "Notification deleted." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting notification {Id}", id);
				return StatusCode(500, new { message = "Error deleting notification." });
			}
		}

		// POST api/notification/{id}/restore
		[HttpPost("{id:long}/restore")]
		public async Task<IActionResult> Restore(
			long id, CancellationToken ct = default)
		{
			try
			{
				var ok = await _service.RestoreAsync(id, ct);
				if (!ok)
					return NotFound(new { message = $"Notification {id} not found." });

				return Ok(new { message = "Notification restored." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error restoring notification {Id}", id);
				return StatusCode(500, new { message = "Error restoring notification." });
			}
		}

		// ── Helper ─────────────────────────────────────
		private long? GetCurrentUserId()
		{
			var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
				   ?? User.FindFirst("sub")?.Value;
			return long.TryParse(raw, out var id) ? id : null;
		}
	}
}