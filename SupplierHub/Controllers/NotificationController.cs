using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SupplierHub.Models;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class NotificationController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly ILogger<NotificationController> _logger;

		public NotificationController(AppDbContext db, ILogger<NotificationController> logger)
		{
			_db = db;
			_logger = logger;
		}

		// GET: api/notifications
		[HttpGet]
		public async Task<IActionResult> GetAll(CancellationToken ct = default)
		{
			try
			{
				var items = await _db.Notifications
					.AsNoTracking()
					.Where(n => !n.IsDeleted)
					.OrderByDescending(n => n.CreatedOn)
					.ToListAsync(ct);

				if (!items.Any())
					return Ok(new { message = "No active notifications found.", data = items });

				return Ok(items);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching notifications.");
				return StatusCode(500, new { message = "An error occurred while retrieving notifications." });
			}
		}

		// GET: api/notifications/{id}
		[HttpGet("{id:long}")]
		public async Task<IActionResult> GetById(long id, CancellationToken ct = default)
		{
			try
			{
				var entity = await _db.Notifications
					.AsNoTracking()
					.FirstOrDefaultAsync(n => n.NotificationID == id, ct);

				if (entity == null || entity.IsDeleted)
					return NotFound(new { message = $"Notification with ID {id} was not found or has been deleted." });

				return Ok(entity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching notification {Id}", id);
				return StatusCode(500, new { message = "An error occurred while fetching the notification details." });
			}
		}

		// POST: api/notifications
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Notification model, CancellationToken ct = default)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { message = "Invalid notification data.", errors = ModelState });

			try
			{
				var userExists = await _db.Users.AnyAsync(u => u.UserID == model.UserID, ct);
				if (!userExists)
					return NotFound(new { message = $"User with ID {model.UserID} does not exist. Cannot create notification." });

				var now = DateTime.UtcNow;
				model.NotificationID = 0;
				if (model.CreatedDate == default) model.CreatedDate = now;
				model.CreatedOn = now;
				model.UpdatedOn = now;
				model.IsDeleted = false;

				_db.Notifications.Add(model);
				await _db.SaveChangesAsync(ct);

				return CreatedAtAction(nameof(GetById), new { id = model.NotificationID }, new { message = "Notification created successfully.", data = model });
			}
			catch (DbUpdateException dbEx)
			{
				_logger.LogError(dbEx, "Database error creating notification.");
				return BadRequest(new { message = "Could not save notification due to a database constraint error." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error creating notification.");
				return StatusCode(500, new { message = "An internal server error occurred during notification creation." });
			}
		}

		

		

		// POST: api/notifications/{id}/restore
		[HttpPost("{id:long}/restore")]
		public async Task<IActionResult> Restore(long id, CancellationToken ct = default)
		{
			try
			{
				var entity = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationID == id, ct);

				if (entity == null)
					return NotFound(new { message = $"Restore failed: No notification exists with ID {id}." });

				if (!entity.IsDeleted)
					return BadRequest(new { message = "Notification is already active and does not need restoring." });

				entity.IsDeleted = false;
				entity.UpdatedOn = DateTime.UtcNow;

				await _db.SaveChangesAsync(ct);
				return Ok(new { message = "Notification restored successfully.", data = entity });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error restoring notification {Id}", id);
				return StatusCode(500, new { message = "An internal error occurred during restoration." });
			}
		}
		// Patch: api/notifications/{id}
		[HttpPatch("{id:long}")]
		public async Task<IActionResult> UpdateStatus(long id, [FromBody] Notification model, CancellationToken ct = default)
		{
			// 1. Validation check
			if (model == null)
				return BadRequest(new { message = "Request body cannot be empty." });

			try
			{
				// 2. Retrieve the existing entity
				var entity = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationID == id, ct);

				if (entity == null)
					return NotFound(new { message = $"Notification with ID {id} not found." });

				// 3. PARTIAL UPDATE LOGIC (The "Patch" way)
				// We only update the Status. We do NOT allow the user to change 
				// the Message, Category, or UserID to maintain audit integrity.
				entity.Status = model.Status; // Statuses: Unread, Read, Dismissed 

				// Update the timestamp for internal tracking
				entity.UpdatedOn = DateTime.UtcNow;

				// 4. Save only the changed fields
				await _db.SaveChangesAsync(ct);

				return Ok(new { message = "Notification status updated successfully.", data = entity });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error patching notification {Id}", id);
				return StatusCode(500, new { message = "An error occurred during the update." });
			}
		}
	}
}
