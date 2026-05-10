using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.CategoryDTO;
using Microsoft.AspNetCore.Authorization;
using SupplierHub.Constants;

namespace SupplierHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ICategoryService _service;

		public CategoriesController(ICategoryService service)
		{
			_service = service;
		}

		/// <summary>
		/// Create category
		/// </summary>
		[HttpPost]
		[Authorize(Roles = nameof(RoleType.Admin) + "," + nameof(RoleType.CategoryManager))]
		[ProducesResponseType(typeof(CategoryGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create(
			[FromBody] CategoryCreateDto dto,
			CancellationToken ct)
		{
			try
			{
				var created = await _service.CreateAsync(dto, ct);
				return Ok(new { message = "Category created successfully.", data = created });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating category.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get category by ID
		/// </summary>
		[HttpGet("{categoryId:long}")]
		[Authorize(Roles =
			nameof(RoleType.Admin) + "," +
			nameof(RoleType.CategoryManager) + "," +
			nameof(RoleType.Buyer) + "," +
			nameof(RoleType.SupplierUser))]
		[ProducesResponseType(typeof(CategoryGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(long categoryId, CancellationToken ct)
		{
			try
			{
				var category = await _service.GetByIdAsync(categoryId, ct);
				if (category == null)
					return NotFound(new { message = $"Category with ID {categoryId} not found." });

				return Ok(new { message = "Category retrieved successfully.", data = category });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving category.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get all categories
		/// </summary>
		[HttpGet]
		[Authorize(Roles =
			nameof(RoleType.Admin) + "," +
			nameof(RoleType.CategoryManager) + "," +
			nameof(RoleType.Buyer) + "," +
			nameof(RoleType.SupplierUser))]
		[ProducesResponseType(typeof(IEnumerable<CategoryGetAllDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			try
			{
				var categories = await _service.GetAllAsync(ct);
				return Ok(new { message = "Categories retrieved successfully.", data = categories });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving categories.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Update category
		/// </summary>
		[HttpPut("{categoryId:long}")]
		[Authorize(Roles = nameof(RoleType.Admin) + "," + nameof(RoleType.CategoryManager))]
		[ProducesResponseType(typeof(CategoryGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			long categoryId,
			[FromBody] CategoryUpdateDto dto,
			CancellationToken ct)
		{
			try
			{
				if (dto.CategoryID != categoryId)
					return BadRequest(new { message = "Category ID mismatch." });

				var updated = await _service.UpdateAsync(dto, ct);
				if (updated == null)
					return NotFound(new { message = $"Category with ID {categoryId} not found." });

				return Ok(new { message = "Category updated successfully.", data = updated });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while updating category.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Soft delete category
		/// </summary>
		[HttpDelete]
		[Authorize(Roles = nameof(RoleType.Admin))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Delete(
			[FromBody] CategoryDeleteDto dto,
			CancellationToken ct)
		{
			try
			{
				var deleted = await _service.DeleteAsync(dto, ct);
				if (!deleted)
					return NotFound(new { message = "No matching category found to delete." });

				return Ok(new { message = "Category deleted successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while deleting category.",
					error = ex.Message
				});
			}
		}
	}
}