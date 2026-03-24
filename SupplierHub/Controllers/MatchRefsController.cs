using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.MatchRefDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MatchRefsController : ControllerBase
	{
		private readonly IMatchRefService _service;

		public MatchRefsController(IMatchRefService service) => _service = service;

		[HttpGet("invoice/{invoiceId}")]
		public async Task<ActionResult<IEnumerable<MatchRefResponseDto>>> GetByInvoiceId(long invoiceId) =>
			Ok(await _service.GetByInvoiceIdAsync(invoiceId));

		[HttpGet("{id}")]
		public async Task<ActionResult<MatchRefResponseDto>> GetById(long id)
		{
			var result = await _service.GetByIdAsync(id);
			if (result == null) return NotFound();
			return Ok(result);
		}

		[HttpPost]
		public async Task<ActionResult<MatchRefResponseDto>> Create([FromBody] MatchRefCreateDto createDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _service.CreateAsync(createDto);
			return CreatedAtAction(nameof(GetById), new { id = result.MatchID }, result);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<MatchRefResponseDto>> Update(long id, [FromBody] MatchRefUpdateDto updateDto)
		{
			// Assuming your MatchRefUpdateDto has MatchID
			// If it uses a different ID property name, swap it below!
			var dtoId = (long)updateDto.GetType().GetProperty("MatchID")?.GetValue(updateDto, null)!;

			if (id != dtoId) return BadRequest("ID mismatch");

			var result = await _service.UpdateAsync(id, updateDto);
			if (result == null) return NotFound();
			return Ok(result);
		}
	}
}