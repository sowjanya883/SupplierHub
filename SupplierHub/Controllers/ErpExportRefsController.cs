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
			var records = await _service.GetAllAsync();
			return Ok(records);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ErpExportRefResponseDto>> GetById(long id)
		{
			var record = await _service.GetByIdAsync(id);
			if (record == null) return NotFound();
			return Ok(record);
		}

		[HttpPost]
		public async Task<ActionResult<ErpExportRefResponseDto>> Create([FromBody] ErpExportRefCreateDto createDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var result = await _service.CreateAsync(createDto);
			return CreatedAtAction(nameof(GetById), new { id = result.ErprefID }, result);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<ErpExportRefResponseDto>> Update(long id, [FromBody] ErpExportRefUpdateDto updateDto)
		{
			if (id != updateDto.ErprefID) return BadRequest("ID mismatch");

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