using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.PoRevisionDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PoRevisionsController : ControllerBase
	{
		private readonly IPoRevisionService _service;

		public PoRevisionsController(IPoRevisionService service)
		{
			_service = service;
		}

		[HttpGet("po/{poId}")]
		public async Task<ActionResult<IEnumerable<PoRevisionResponseDto>>> GetByPoId(long poId)
		{
			var revisions = await _service.GetAllByPoIdAsync(poId);
			return Ok(revisions);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<PoRevisionResponseDto>> GetById(long id)
		{
			var revision = await _service.GetByIdAsync(id);
			if (revision == null) return NotFound();
			return Ok(revision);
		}

		[HttpPost]
		public async Task<ActionResult<PoRevisionResponseDto>> Create([FromBody] PoRevisionCreateDto createDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var result = await _service.CreateAsync(createDto);
			return CreatedAtAction(nameof(GetById), new { id = result.PorevID }, result);
		}
	}
}