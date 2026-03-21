using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.SupplierRiskDTO;

namespace SupplierHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SupplierRisksController : ControllerBase
	{
		private readonly ISupplierRiskService _service;

		public SupplierRisksController(ISupplierRiskService service)
		{
			_service = service;
		}

		/// <summary>
		/// Create supplier risk
		/// </summary>
		[HttpPost]
		[ProducesResponseType(typeof(SupplierRiskGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create(
			[FromBody] SupplierRiskCreateDto dto,
			CancellationToken ct)
		{
			try
			{

				var created = await _service.CreateAsync(dto, ct);

				return Ok(new { message = "Supplier risk created successfully.", data = created });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating supplier risk.",
					error = ex.InnerException?.Message ?? ex.Message
				});
			}

		}

		/// <summary>
		/// Get supplier risk by ID
		/// </summary>
		[HttpGet("{riskId:long}")]
		[ProducesResponseType(typeof(SupplierRiskGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(long riskId, CancellationToken ct)
		{
			try
			{
				var risk = await _service.GetByIdAsync(riskId, ct);
				if (risk == null)
					return NotFound(new { message = $"Risk with ID {riskId} not found." });

				return Ok(new { message = "Supplier risk retrieved successfully.", data = risk });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving supplier risk.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get all supplier risks
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<SupplierRiskGetAllDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			try
			{
				var risks = await _service.GetAllAsync(ct);
				return Ok(new { message = "Supplier risks retrieved successfully.", data = risks });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving supplier risks.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Update supplier risk
		/// </summary>
		[HttpPut("{riskId:long}")]
		[ProducesResponseType(typeof(SupplierRiskGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			long riskId,
			[FromBody] SupplierRiskUpdateDto dto,
			CancellationToken ct)
		{
			try
			{
				if (dto.RiskID != riskId)
					return BadRequest(new { message = "Risk ID mismatch." });

				var updated = await _service.UpdateAsync(dto, ct);
				if (updated == null)
					return NotFound(new { message = $"Risk with ID {riskId} not found." });

				return Ok(new { message = "Supplier risk updated successfully.", data = updated });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while updating supplier risk.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Soft delete supplier risk
		/// </summary>
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Delete(
			[FromBody] SupplierRiskDeleteDto dto,
			CancellationToken ct)
		{
			try
			{
				var deleted = await _service.DeleteAsync(dto, ct);
				if (!deleted)
					return NotFound(new { message = "No matching supplier risk found to delete." });

				return Ok(new { message = "Supplier risk deleted successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while deleting supplier risk.",
					error = ex.Message
				});
			}
		}
	}
}