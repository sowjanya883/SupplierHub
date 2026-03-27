using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.ErpExportRefDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "AccountsPayable, Admin")]
	public class ErpExportRefsController : ControllerBase
	{
		private readonly IErpExportRefService _service;

		public ErpExportRefsController(IErpExportRefService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ErpExportRefResponseDto>>> GetAll()
		{
			try
			{
				var records = await _service.GetAllAsync();
				return Ok(records);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ErpExportRefResponseDto>> GetById(long id)
		{
			try
			{
				var record = await _service.GetByIdAsync(id);

				if (record == null)
					return NotFound();

				return Ok(record);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPost]
		public async Task<ActionResult<ErpExportRefResponseDto>> Create([FromBody] ErpExportRefCreateDto createDto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				if (createDto == null)
					return BadRequest("ERP Export Ref data is null.");

				var result = await _service.CreateAsync(createDto);

				// Meticulously maintained your 'ErprefID' casing
				return CreatedAtAction(nameof(GetById), new { id = result.ErprefID }, result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<ErpExportRefResponseDto>> Update(long id, [FromBody] ErpExportRefUpdateDto updateDto)
		{
			try
			{
				if (updateDto == null)
					return BadRequest("Update data is null.");

				// Meticulously maintained your 'ErprefID' casing
				if (id != updateDto.ErprefID)
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