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
			try
			{
				var result = await _service.GetAllAsync();
				return Ok(result);
			}
			catch (Exception ex)
			{
				// Returns a safe 500 error to the frontend if something breaks
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<PurchaseOrderResponseDto>> GetById(long id)
		{
			try
			{
				var order = await _service.GetByIdAsync(id);

				if (order == null)
					return NotFound();

				return Ok(order);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPost]
		public async Task<ActionResult<PurchaseOrderResponseDto>> Create([FromBody] PurchaseOrderCreateDto createDto)
		{
			try
			{
				if (createDto == null)
					return BadRequest("Purchase Order data is null.");

				var result = await _service.CreateAsync(createDto);
				return CreatedAtAction(nameof(GetById), new { id = result.PoID }, result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<PurchaseOrderResponseDto>> Update(long id, [FromBody] PurchaseOrderUpdateDto updateDto)
		{
			try
			{
				if (updateDto == null)
					return BadRequest("Update data is null.");

				if (id != updateDto.PoID)
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