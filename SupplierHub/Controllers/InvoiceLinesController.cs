using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierHub.DTOs.InvoiceLineDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,SupplierUser,AccountsPayable")]
	public class InvoiceLinesController : ControllerBase
	{
		private readonly IInvoiceLineService _service;

		public InvoiceLinesController(IInvoiceLineService service)
		{
			_service = service;
		}

		[HttpGet("invoice/{invoiceId}")]
		public async Task<ActionResult<IEnumerable<InvoiceLineResponseDto>>> GetByInvoiceId(long invoiceId)
		{
			try
			{
				var result = await _service.GetByInvoiceIdAsync(invoiceId);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<InvoiceLineResponseDto>> GetById(long id)
		{
			try
			{
				var result = await _service.GetByIdAsync(id);

				if (result == null)
					return NotFound();

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPost]
		public async Task<ActionResult<InvoiceLineResponseDto>> Create([FromBody] InvoiceLineCreateDto createDto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(new { message = "Invalid invoice line payload — check required fields.", errors = ModelState });

				if (createDto == null)
					return BadRequest(new { message = "Invoice line data is null." });

				var result = await _service.CreateAsync(createDto);
				return CreatedAtAction(nameof(GetById), new { id = result.InvLineID }, result);
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(new { message = "Could not create invoice line — check that Invoice ID and PO Line ID exist.", detail = ex.InnerException?.Message ?? ex.Message });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "Could not create invoice line.", detail = ex.Message });
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<InvoiceLineResponseDto>> Update(long id, [FromBody] InvoiceLineUpdateDto updateDto)
		{
			try
			{
				if (updateDto == null)
					return BadRequest("Update data is null.");

				// Meticulously maintained your 'InvLineID' casing
				if (id != updateDto.InvLineID)
					return BadRequest("ID mismatch between route and payload.");

				var result = await _service.UpdateAsync(id, updateDto);

				if (result == null)
					return NotFound();

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}