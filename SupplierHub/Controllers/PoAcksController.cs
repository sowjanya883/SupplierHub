using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.PoAckDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PoAcksController : ControllerBase
	{
		private readonly IPoAckService _service;

		public PoAcksController(IPoAckService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<PoAckResponseDto>>> GetAll()
		{
			return Ok(await _service.GetAllAsync());
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<PoAckResponseDto>> GetById(long id)
		{
			var ack = await _service.GetByIdAsync(id);
			if (ack == null) return NotFound();
			return Ok(ack);
		}

		[HttpPost]
		public async Task<ActionResult<PoAckResponseDto>> Create([FromBody] PoAckCreateDto
		createDto)
		{
			try
			{
				var result = await _service.CreateAsync(createDto);
				return CreatedAtAction(nameof(GetById), new { id = result.PocfmID }, result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<PoAckResponseDto>> Update(long id, [FromBody] PoAckUpdateDto updateDto)
		{
			if (id != updateDto.PocfmID) return BadRequest("ID mismatch");

			try
			{
				var result = await _service.UpdateAsync(id, updateDto);
				if (result == null) return NotFound();
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}