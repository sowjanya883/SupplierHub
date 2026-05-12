using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.MatchRefDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,AccountsPayable,Buyer,ReceivingUser")]
	public class MatchRefsController : ControllerBase
	{
		private readonly IMatchRefService _service;

		public MatchRefsController(IMatchRefService service)
		{
			_service = service;
		}

		[HttpGet("invoice/{invoiceId}")]
		public async Task<ActionResult<IEnumerable<MatchRefResponseDto>>> GetByInvoiceId(long invoiceId)
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
		public async Task<ActionResult<MatchRefResponseDto>> GetById(long id)
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
		public async Task<ActionResult<MatchRefResponseDto>> Create([FromBody] MatchRefCreateDto createDto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				if (createDto == null)
					return BadRequest("Match Ref data is null.");

				var result = await _service.CreateAsync(createDto);

				// Meticulously maintained your 'MatchID' casing
				return CreatedAtAction(nameof(GetById), new { id = result.MatchID }, result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<MatchRefResponseDto>> Update(long id, [FromBody] MatchRefUpdateDto updateDto)
		{
			try
			{
				if (updateDto == null)
					return BadRequest("Update data is null.");

				// Maintained your reflection logic, but added a safety net so it doesn't crash 
				// if the property name ever changes in the future!
				var propertyInfo = updateDto.GetType().GetProperty("MatchID");
				if (propertyInfo == null)
					return BadRequest("MatchID property not found on update payload.");

				var dtoId = (long)propertyInfo.GetValue(updateDto, null)!;

				if (id != dtoId)
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