using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SupplierHub.DTOs.CatalogDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CatalogController : ControllerBase
	{
		private readonly ICatalogService _service;

		public CatalogController(ICatalogService service)
		{
			_service = service;
		}

		// CREATE
		[HttpPost]
		[ProducesResponseType(typeof(CatalogCreateDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create(
			[FromBody] CatalogCreateDto dto,
			CancellationToken ct)
		{
			if (dto == null)
				return BadRequest("Request body is required.");

			var result = await _service.CreateAsync(dto, ct);
			return Ok(result);
		}

		// GET ALL
		[HttpGet]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			var result = await _service.GetAllAsync(ct);
			return Ok(result);
		}

		// GET BY ID
		[HttpGet("{id:long}")]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(long id, CancellationToken ct)
		{
			var result = await _service.GetByIdAsync(id, ct);
			if (result == null)
				return NotFound("Catalog not found.");

			return Ok(result);
		}

		// UPDATE
		[HttpPut("{id:long}")]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			long id,
			[FromBody] CatalogUpdateDto dto,
			CancellationToken ct)
		{
			if (dto == null || id != dto.CatalogID)
				return BadRequest("Invalid request data.");

			var result = await _service.UpdateAsync(dto, ct);
			if (result == null)
				return NotFound("Catalog not found.");

			return Ok(result);
		}

		// DELETE
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Delete(
			[FromBody] CatalogDeleteDto dto,
			CancellationToken ct)
		{
			if (dto == null)
				return BadRequest("Request body is required.");

			var success = await _service.DeleteAsync(dto, ct);
			if (!success)
				return BadRequest("Unable to delete catalog.");

			return Ok("Catalog deleted successfully.");
		}
	}
}