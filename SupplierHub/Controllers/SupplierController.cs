using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.DTOs.OrganizationDTO;
using SupplierHub.DTOs.SupplierContactDTO;
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.Services;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SupplierController : ControllerBase
	{
		private readonly ISupplierService _service;

		public SupplierController(ISupplierService service)
		{
			_service = service;
		}

		/// <summary>
		/// Create a new supplier
		/// </summary>
		/// <param name="dto">Supplier creation data</param>
		/// <returns>Created supplier DTO</returns>
		/// <response code="200">Supplier created successfully</response>
		/// <response code="400">Invalid request data</response>
		/// <response code="409">Supplier already exists</response>
		/// <response code="500">Server error</response>
		[HttpPost]
		[ProducesResponseType(typeof(GetSupplierByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create([FromBody] SupplierCreateDto dto, CancellationToken ct)
		{
			try
			{
				var created = await _service.CreateAsync(dto, ct);
				return Ok(new { message = "Supplier created successfully.", data = created });
			}
			catch (KeyNotFoundException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating the supplier.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get supplier by ID
		/// </summary>
		/// <param name="supplierId">Supplier ID</param>
		/// <returns>Supplier DTO</returns>
		/// <response code="200">Supplier found</response>
		/// <response code="404">Supplier not found</response>
		/// <response code="500">Server error</response>
		[HttpGet("{supplierId:long}")]
		[ProducesResponseType(typeof(GetSupplierByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetById(long supplierId, CancellationToken ct)
		{
			try
			{
				var supplier = await _service.GetByIdAsync(supplierId, ct);
				if (supplier == null)
					return NotFound(new { message = $"Supplier with ID {supplierId} not found." });

				return Ok(new { message = "Supplier retrieved successfully.", data = supplier });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving the supplier.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get all suppliers
		/// </summary>
		/// <returns>List of supplier DTOs</returns>
		/// <response code="200">Suppliers retrieved successfully</response>
		/// <response code="500">Server error</response>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<GetAllSupplierDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			try
			{
				var suppliers = await _service.GetAllAsync(ct);
				return Ok(new { message = "Suppliers retrieved successfully.", data = suppliers });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving suppliers.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Update supplier
		/// </summary>
		/// <param name="supplierId">Supplier ID</param>
		/// <param name="dto">Supplier update data</param>
		/// <returns>Updated supplier DTO</returns>
		/// <response code="200">Supplier updated successfully</response>
		/// <response code="404">Supplier not found</response>
		/// <response code="400">Invalid request data</response>
		/// <response code="500">Server error</response>
		[HttpPut("{supplierId:long}")]
		[ProducesResponseType(typeof(GetSupplierByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Update(
			long supplierId,
			[FromBody] UpdateSupplierDto dto,
			CancellationToken ct)
		{
			try
			{
				if (dto.SupplierID != supplierId)
					return BadRequest(new { message = "Supplier ID mismatch." });

				var updated = await _service.UpdateAsync(dto, ct);
				if (updated == null)
					return NotFound(new { message = $"Supplier with ID {supplierId} not found." });

				return Ok(new { message = "Supplier updated successfully.", data = updated });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while updating the supplier.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Soft delete a supplier
		/// </summary>
		/// <param name="dto">Supplier identifier(s)</param>
		/// <returns>Deletion status</returns>
		/// <response code="200">Supplier deleted successfully</response>
		/// <response code="400">Invalid deletion criteria</response>
		/// <response code="500">Server error</response>
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Delete([FromBody] SupplierDeleteDto dto, CancellationToken ct)
		{
			try
			{
				var deleted = await _service.DeleteAsync(dto, ct);
				if (!deleted)
					return NotFound(new { message = "No matching supplier found to delete." });

				return Ok(new { message = "Supplier deleted successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while deleting the supplier.",
					error = ex.Message
				});
			}
		}
	}
}