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
			var lines = await _service.GetAllByPoIdAsync(poId);
			return Ok(lines);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<PoLineResponseDto>> GetById(long id)
		{
			var line = await _service.GetByIdAsync(id);
			if (line == null) return NotFound();
			return Ok(line);
		}

		[HttpPost]
		public async Task<ActionResult<PoLineResponseDto>> Create([FromBody] PoLineCreateDto createDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var result = await _service.CreateAsync(createDto);
			return CreatedAtAction(nameof(GetById), new { id = result.PoLineId }, result);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<PoLineResponseDto>> Update(long id, [FromBody] PoLineUpdateDto updateDto)
		{
			if (id != updateDto.PoLineId) return BadRequest("ID mismatch");

			var result = await _service.UpdateAsync(id, updateDto);
			if (result == null) return NotFound();

			return Ok(result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(long id)
		{
			var success = await _service.DeleteAsync(id);
			if (!success) return NotFound();
			return NoContent();
		}
	}
}