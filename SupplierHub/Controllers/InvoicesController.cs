using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.InvoiceDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class InvoicesController : ControllerBase
	{
		private readonly IInvoiceService _service;

		public InvoicesController(IInvoiceService service) => _service = service;

		[HttpGet]
		public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetAll() => Ok(await _service.GetAllAsync());

		[HttpGet("{id}")]
		public async Task<ActionResult<InvoiceResponseDto>> GetById(long id)
		{
			var result = await _service.GetByIdAsync(id);
			if (result == null) return NotFound();
			return Ok(result);
		}

		[HttpGet("po/{poId}")]
		public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetByPoId(long poId) =>
			Ok(await _service.GetByPoIdAsync(poId));

		[HttpPost]
		public async Task<ActionResult<InvoiceResponseDto>> Create([FromBody] InvoiceCreateDto createDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _service.CreateAsync(createDto);
			return CreatedAtAction(nameof(GetById), new { id = result.InvoiceID }, result);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<InvoiceResponseDto>> Update(long id, [FromBody] InvoiceUpdateDto updateDto)
		{
			if (id != updateDto.InvoiceID) return BadRequest("ID mismatch");
			var result = await _service.UpdateAsync(id, updateDto);
			if (result == null) return NotFound();
			return Ok(result);
		}
	}
}