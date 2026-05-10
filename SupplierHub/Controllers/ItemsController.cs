using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.ItemDTO;
using Microsoft.AspNetCore.Authorization;
using SupplierHub.Constants;

namespace SupplierHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ItemsController : ControllerBase
	{
		private readonly IItemService _service;

		public ItemsController(IItemService service)
		{
			_service = service;
		}

		/// <summary>
		/// Create item
		/// </summary>
		[HttpPost]
		[Authorize(Roles = nameof(RoleType.Admin) + "," + nameof(RoleType.CategoryManager))]
		[ProducesResponseType(typeof(ItemGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create(
			[FromBody] ItemCreateDto dto,
			CancellationToken ct)
		{
			try
			{
				var created = await _service.CreateAsync(dto, ct);
				return Ok(new { message = "Item created successfully.", data = created });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating item.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get item by ID
		/// </summary>
		[HttpGet("{itemId:long}")]
		[Authorize(Roles =
			nameof(RoleType.Admin) + "," +
			nameof(RoleType.CategoryManager) + "," +
			nameof(RoleType.Buyer) + "," +
			nameof(RoleType.SupplierUser))]
		[ProducesResponseType(typeof(ItemGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(long itemId, CancellationToken ct)
		{
			try
			{
				var item = await _service.GetByIdAsync(itemId, ct);
				if (item == null)
					return NotFound(new { message = $"Item with ID {itemId} not found." });

				return Ok(new { message = "Item retrieved successfully.", data = item });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving item.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get all items
		/// </summary>
		[HttpGet]
		[Authorize(Roles =
			nameof(RoleType.Admin) + "," +
			nameof(RoleType.CategoryManager) + "," +
			nameof(RoleType.Buyer) + "," +
			nameof(RoleType.SupplierUser))]
		[ProducesResponseType(typeof(IEnumerable<ItemGetAllDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			try
			{
				var items = await _service.GetAllAsync(ct);
				return Ok(new { message = "Items retrieved successfully.", data = items });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving items.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Update item
		/// </summary>
		[HttpPut("{itemId:long}")]
		[Authorize(Roles = nameof(RoleType.Admin) + "," + nameof(RoleType.CategoryManager))]
		[ProducesResponseType(typeof(ItemGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			long itemId,
			[FromBody] ItemUpdateDto dto,
			CancellationToken ct)
		{
			try
			{
				if (dto.ItemID != itemId)
					return BadRequest(new { message = "Item ID mismatch." });

				var updated = await _service.UpdateAsync(dto, ct);
				if (updated == null)
					return NotFound(new { message = $"Item with ID {itemId} not found." });

				return Ok(new { message = "Item updated successfully.", data = updated });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while updating item.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Soft delete item
		/// </summary>
		[HttpDelete]
		[Authorize(Roles = nameof(RoleType.Admin))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Delete(
			[FromBody] ItemDeleteDto dto,
			CancellationToken ct)
		{
			try
			{
				var deleted = await _service.DeleteAsync(dto, ct);
				if (!deleted)
					return NotFound(new { message = "Item not found." });

				return Ok(new { message = "Item deleted successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while deleting item.",
					error = ex.Message
				});
			}
		}
	}
}