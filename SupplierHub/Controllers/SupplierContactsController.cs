using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.SupplierContactDTO;

namespace SupplierHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SupplierContactsController : ControllerBase
	{
		private readonly ISupplierContactService _service;

		public SupplierContactsController(ISupplierContactService service)
		{
			_service = service;
		}

		/// <summary>
		/// Create supplier contact
		/// </summary>
		[HttpPost]
		[ProducesResponseType(typeof(SupplierContactGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create(
			[FromBody] SupplierContactCreateDto dto,
			CancellationToken ct)
		{
			try
			{
				var created = await _service.CreateAsync(dto, ct);
				return Ok(new { message = "Supplier contact created successfully.", data = created });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating supplier contact.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get supplier contact by ID
		/// </summary>
		[HttpGet("{contactId:long}")]
		[ProducesResponseType(typeof(SupplierContactGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(long contactId, CancellationToken ct)
		{
			try
			{
				var contact = await _service.GetByIdAsync(contactId, ct);
				if (contact == null)
					return NotFound(new { message = $"Contact with ID {contactId} not found." });

				return Ok(new { message = "Supplier contact retrieved successfully.", data = contact });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving supplier contact.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get all supplier contacts
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<SupplierContactGetAllDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			try
			{
				var contacts = await _service.GetAllAsync(ct);
				return Ok(new { message = "Supplier contacts retrieved successfully.", data = contacts });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving supplier contacts.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Update supplier contact
		/// </summary>
		[HttpPut("{contactId:long}")]
		[ProducesResponseType(typeof(SupplierContactGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			long contactId,
			[FromBody] SupplierContactUpdateDto dto,
			CancellationToken ct)
		{
			try
			{
				if (dto.ContactID != contactId)
					return BadRequest(new { message = "Contact ID mismatch." });

				var updated = await _service.UpdateAsync(dto, ct);
				if (updated == null)
					return NotFound(new { message = $"Contact with ID {contactId} not found." });

				return Ok(new { message = "Supplier contact updated successfully.", data = updated });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while updating supplier contact.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Soft delete supplier contact
		/// </summary>
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Delete(
			[FromBody] SupplierContactDeleteDto dto,
			CancellationToken ct)
		{
			try
			{
				var deleted = await _service.DeleteAsync(dto, ct);
				if (!deleted)
					return NotFound(new { message = "No matching supplier contact found to delete." });

				return Ok(new { message = "Supplier contact deleted successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while deleting supplier contact.",
					error = ex.Message
				});
			}
		}
	}
}
