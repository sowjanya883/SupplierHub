using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.PurchaseOrderDTO;
using SupplierHub.Services;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin, SupplierUser, CategoryManager, Buyer")]
	public class PurchaseOrdersController : ControllerBase
	{
		private readonly IPurchaseOrderService _service;

		public PurchaseOrdersController(IPurchaseOrderService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<PurchaseOrderResponseDto>>> GetAll()
		{
			return Ok(await _service.GetAllAsync());
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<PurchaseOrderResponseDto>> GetById(long id)
		{
			var order = await _service.GetByIdAsync(id);
			if (order == null) return NotFound();
			return Ok(order);
		}

		[HttpPost]
		public async Task<ActionResult<PurchaseOrderResponseDto>> Create([FromBody] PurchaseOrderCreateDto createDto)
		{
			var result = await _service.CreateAsync(createDto);
			return CreatedAtAction(nameof(GetById), new { id = result.PoID }, result);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<PurchaseOrderResponseDto>> Update(long id, [FromBody] PurchaseOrderUpdateDto updateDto)
		{
			if (id != updateDto.PoID) return BadRequest("ID mismatch");

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