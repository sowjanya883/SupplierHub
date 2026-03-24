using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.InvoiceLineDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class InvoiceLinesController : ControllerBase
	{
		private readonly IInvoiceLineService _service;

		public InvoiceLinesController(IInvoiceLineService service) => _service = service;

		[HttpGet("invoice/{invoiceId}")]
		public async Task<ActionResult<IEnumerable<InvoiceLineResponseDto>>> GetByInvoiceId(long invoiceId) =>
			Ok(await _service.GetByInvoiceIdAsync(invoiceId));

		[HttpGet("{id}")]
		public async Task<ActionResult<InvoiceLineResponseDto>> GetById(long id)
		{
			var result = await _service.GetByIdAsync(id);
			if (result == null) return NotFound();
			return Ok(result);
		}

		[HttpPost]
		public async Task<ActionResult<InvoiceLineResponseDto>> Create([FromBody] InvoiceLineCreateDto createDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _service.CreateAsync(createDto);
			return CreatedAtAction(nameof(GetById), new { id = result.InvLineID }, result);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<InvoiceLineResponseDto>> Update(long id, [FromBody] InvoiceLineUpdateDto updateDto)
		{
			if (id != updateDto.InvLineID) return BadRequest("ID mismatch");
			var result = await _service.UpdateAsync(id, updateDto);
			if (result == null) return NotFound();
			return Ok(result);
		}
	}
}