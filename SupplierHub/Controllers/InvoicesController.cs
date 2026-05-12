using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.InvoiceDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,SupplierUser,AccountsPayable,Buyer")]
	public class InvoicesController : ControllerBase
	{
		private readonly IInvoiceService _service;

		public InvoicesController(IInvoiceService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetAll()
		{
			try
			{
				var result = await _service.GetAllAsync();
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<InvoiceResponseDto>> GetById(long id)
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

		[HttpGet("po/{poId}")]
		public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetByPoId(long poId)
		{
			try
			{
				var result = await _service.GetByPoIdAsync(poId);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPost]
		public async Task<ActionResult<InvoiceResponseDto>> Create([FromBody] InvoiceCreateDto createDto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				if (createDto == null)
					return BadRequest("Invoice data is null.");

				var result = await _service.CreateAsync(createDto);

				// Meticulously maintained your 'InvoiceID' casing
				return CreatedAtAction(nameof(GetById), new { id = result.InvoiceID }, result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<InvoiceResponseDto>> Update(long id, [FromBody] InvoiceUpdateDto updateDto)
		{
			try
			{
				if (updateDto == null)
					return BadRequest("Update data is null.");

				// Meticulously maintained your 'InvoiceID' casing
				if (id != updateDto.InvoiceID)
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