using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupplierHub.DTOs.AuditLogDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuditLogController : ControllerBase
	{
		private readonly IAuditLogService _service;
		private readonly ILogger<AuditLogController> _logger;

		public AuditLogController(IAuditLogService service, ILogger<AuditLogController> logger)
		{
			_service = service;
			_logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false, CancellationToken ct = default)
		{
			try
			{
				var items = await _service.GetAllAsync(includeDeleted, ct);

				if (items == null)
					return Ok(new { message = "No audit logs found.", data = Array.Empty<object>() });

				return Ok(items);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving audit logs.");
				return StatusCode(500, new { message = "An error occurred while fetching the audit trail." });
			}
		}

		[HttpGet("{id:long}")]
		public async Task<IActionResult> GetById(long id, CancellationToken ct = default)
		{
			try
			{
				var item = await _service.GetByIdAsync(id, ct);
				if (item == null)
					return NotFound(new { message = $"Audit log entry with ID {id} was not found." });

				return Ok(item);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving audit log {AuditId}", id);
				return StatusCode(500, new { message = $"An internal error occurred while fetching audit log {id}." });
			}
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateAuditLogDto dto, CancellationToken ct = default)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { message = "Audit log data is invalid.", errors = ModelState });

			try
			{
				var created = await _service.CreateAsync(dto, ct);
				return CreatedAtAction(nameof(GetById), new { id = created.AuditID }, new { message = "Audit log entry created successfully.", data = created });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating audit log");
				return StatusCode(500, new { message = "The system failed to record the audit log entry." });
			}
		}

		

		
	}
}