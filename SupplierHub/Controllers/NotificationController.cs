using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		// Get all notifications (excluding soft-deleted by default)
		[HttpGet]
		public async Task<IActionResult> GetAll(CancellationToken ct = default)
		{
			var items = await _db.Notifications
				.AsNoTracking()
				.Where(n => !n.IsDeleted)
				.OrderByDescending(n => n.CreatedOn)
				.ToListAsync(ct);

			return Ok(items);
		}

		// GET: api/notifications/{id}
		// Get notification by id
		[HttpGet("{id:long}")]
		public async Task<IActionResult> GetById(long id, CancellationToken ct = default)
		{
			var entity = await _db.Notifications
				.AsNoTracking()
				.FirstOrDefaultAsync(n => n.NotificationID == id, ct);

			if (entity == null || entity.IsDeleted)
				return NotFound();

			return Ok(entity);
		}

		// POST: api/notifications
		// Create a new notification
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Notification model, CancellationToken ct = default)
		{
			try
			{
				if (!ModelState.IsValid)
					return ValidationProblem(ModelState);

				// Server-side timestamps and defaults
				var now = DateTime.UtcNow;
				model.NotificationID = 0; // ensure new
				if (model.CreatedDate == default) model.CreatedDate = now;
				model.CreatedOn = now;
				model.UpdatedOn = now;
				model.IsDeleted = false;

				// Ensure referenced user exists to avoid FK/constraint errors
				var userExists = await _db.Users.AnyAsync(u => u.UserID == model.UserID, ct);
				if (!userExists)
					return BadRequest($"User with id {model.UserID} does not exist.");

				_db.Notifications.Add(model);
				await _db.SaveChangesAsync(ct);
				return Ok(model);
			}
			catch (DbUpdateException dbEx)
			{
				_logger.LogError(dbEx, "DbUpdateException while creating Notification for UserID {UserId}", model?.UserID);
				var detail = dbEx.InnerException?.Message ?? dbEx.Message;
				return Problem(detail, statusCode: 400);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected exception while creating Notification for UserID {UserId}", model?.UserID);
				var detail = ex.InnerException?.Message ?? ex.Message;
				return Problem(detail, statusCode: 500);
			}
		}

		// PUT: api/notifications/{id}
		// Update an existing notification (full update)
		[HttpPut("{id:long}")]
		public async Task<IActionResult> Update(long id, [FromBody] Notification model, CancellationToken ct = default)
		{
			if (!ModelState.IsValid)
				return ValidationProblem(ModelState);

			var entity = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationID == id, ct);
			if (entity == null || entity.IsDeleted)
				return NotFound();

			// Update allowed fields
			entity.Message = model.Message;
			entity.Category = model.Category;
			entity.RefEntityID = model.RefEntityID;
			entity.Status = model.Status;

			// Typically UserID, CreatedOn/CreatedDate are immutable post-create.
			// Keep them as-is; only bump UpdatedOn.
			entity.UpdatedOn = DateTime.UtcNow;

			await _db.SaveChangesAsync(ct);
			return Ok(entity);
		}

		// DELETE: api/notifications/{id}
		// Soft-delete a notification
		[HttpDelete("{id:long}")]
		public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
		{
			var entity = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationID == id, ct);
			if (entity == null || entity.IsDeleted)
				return NotFound();

			entity.IsDeleted = true;
			entity.UpdatedOn = DateTime.UtcNow;

			await _db.SaveChangesAsync(ct);
			return Ok();
		}

		// POST: api/notifications/{id}/restore
		// Restore a soft-deleted notification
		[HttpPost("{id:long}/restore")]				
		public async Task<IActionResult> Restore(long id, CancellationToken ct = default)
		{
			var entity = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationID == id, ct);
			if (entity == null)
				return NotFound();

			if (!entity.IsDeleted)
				return BadRequest("Notification is not deleted.");

			entity.IsDeleted = false;
			entity.UpdatedOn = DateTime.UtcNow;

			await _db.SaveChangesAsync(ct);
			return Ok(entity);
		}
	}
}
