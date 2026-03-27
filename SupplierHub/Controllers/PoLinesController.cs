using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.PoLineDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin, Buyer, CategoryManager, SupplierUser")]
	public class PoLinesController : ControllerBase
	{
		private readonly IPoLineService _service;

		public PoLinesController(IPoLineService service)
		{
			_service = service;
		}

		[HttpGet("po/{poId}")]
		public async Task<ActionResult<IEnumerable<PoLineResponseDto>>> GetByPoId(long poId)
		{
			try
			{
				var lines = await _service.GetAllByPoIdAsync(poId);
				return Ok(lines);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<PoLineResponseDto>> GetById(long id)
		{
			try
			{
				var line = await _service.GetByIdAsync(id);

				if (line == null)
					return NotFound();

				return Ok(line);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPost]
		public async Task<ActionResult<PoLineResponseDto>> Create([FromBody] PoLineCreateDto createDto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				if (createDto == null)
					return BadRequest("PO Line data is null.");

				var result = await _service.CreateAsync(createDto);
				return CreatedAtAction(nameof(GetById), new { id = result.PoLineId }, result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<PoLineResponseDto>> Update(long id, [FromBody] PoLineUpdateDto updateDto)
		{
			try
			{
				if (updateDto == null)
					return BadRequest("Update data is null.");

				if (id != updateDto.PoLineId)
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

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(long id)
		{
			try
			{
				var success = await _service.DeleteAsync(id);

				if (!success)
					return NotFound();

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}