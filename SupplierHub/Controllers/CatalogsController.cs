using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.CatalogDTO;

namespace SupplierHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CatalogsController : ControllerBase
	{
		private readonly ICatalogService _service;

		public CatalogsController(ICatalogService service)
		{
			_service = service;
		}

		/// <summary>
		/// Create catalog item
		/// </summary>
		[HttpPost]
		[ProducesResponseType(typeof(CatalogGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create(
			[FromBody] CatalogCreateDto dto,
			CancellationToken ct)
		{
			try
			{
				var created = await _service.CreateAsync(dto, ct);
				return Ok(new { message = "Catalog item created successfully.", data = created });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating catalog item.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get catalog item by ID
		/// </summary>
		[HttpGet("{itemId:long}")]
		[ProducesResponseType(typeof(CatalogGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(long itemId, CancellationToken ct)
		{
			try
			{
				var item = await _service.GetByIdAsync(itemId, ct);
				if (item == null)
					return NotFound(new { message = $"Catalog item with ID {itemId} not found." });

				return Ok(new { message = "Catalog item retrieved successfully.", data = item });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving catalog item.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get all catalog items
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<CatalogGetAllDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			try
			{
				var items = await _service.GetAllAsync(ct);
				return Ok(new { message = "Catalog items retrieved successfully.", data = items });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving catalog items.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Update catalog item
		/// </summary>
		[HttpPut("{itemId:long}")]
		[ProducesResponseType(typeof(CatalogGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			long itemId,
			[FromBody] CatalogUpdateDto dto,
			CancellationToken ct)
		{
			try
			{
				if (dto.ItemID != itemId)
					return BadRequest(new { message = "Catalog item ID mismatch." });

				var updated = await _service.UpdateAsync(dto, ct);
				if (updated == null)
					return NotFound(new { message = $"Catalog item with ID {itemId} not found." });

				return Ok(new { message = "Catalog item updated successfully.", data = updated });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while updating catalog item.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Soft delete catalog item
		/// </summary>
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Delete(
			[FromBody] CatalogDeleteDto dto,
			CancellationToken ct)
		{
			try
			{
				var deleted = await _service.DeleteAsync(dto, ct);
				if (!deleted)
					return NotFound(new { message = "No matching catalog item found to delete." });

				return Ok(new { message = "Catalog item deleted successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while deleting catalog item.",
					error = ex.Message
				});
			}
		}
	}
}