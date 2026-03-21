using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.OrganizationDTO;

namespace SupplierHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrganizationsController : ControllerBase
	{
		private readonly IOrganizationService _service;

		public OrganizationsController(IOrganizationService service)
		{
			_service = service;
		}

		/// <summary>
		/// Create organization
		/// </summary>
		[HttpPost]
		[ProducesResponseType(typeof(OrganizationGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create(
			[FromBody] OrganizationCreateDto dto,
			CancellationToken ct)
		{
			try
			{
				var created = await _service.CreateAsync(dto, ct);
				return Ok(new { message = "Organization created successfully.", data = created });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating organization.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get organization by ID
		/// </summary>
		[HttpGet("{orgId:long}")]
		[ProducesResponseType(typeof(OrganizationGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(long orgId, CancellationToken ct)
		{
			try
			{
				var org = await _service.GetByIdAsync(orgId, ct);
				if (org == null)
					return NotFound(new { message = $"Organization with ID {orgId} not found." });

				return Ok(new { message = "Organization retrieved successfully.", data = org });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving organization.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get all organizations
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<OrganizationGetAllDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			try
			{
				var orgs = await _service.GetAllAsync(ct);
				return Ok(new { message = "Organizations retrieved successfully.", data = orgs });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving organizations.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Update organization
		/// </summary>
		[HttpPut("{orgId:long}")]
		[ProducesResponseType(typeof(OrganizationGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			long orgId,
			[FromBody] OrganizationUpdateDto dto,
			CancellationToken ct)
		{
			try
			{
				if (dto.OrgID != orgId)
					return BadRequest(new { message = "Organization ID mismatch." });

				var updated = await _service.UpdateAsync(dto, ct);
				if (updated == null)
					return NotFound(new { message = $"Organization with ID {orgId} not found." });

				return Ok(new { message = "Organization updated successfully.", data = updated });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while updating organization.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Soft delete organization
		/// </summary>
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Delete(
			[FromBody] OrganizationDeleteDto dto,
			CancellationToken ct)
		{
			try
			{
				var deleted = await _service.DeleteAsync(dto, ct);
				if (!deleted)
					return NotFound(new { message = "No matching organization found to delete." });

				return Ok(new { message = "Organization deleted successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while deleting organization.",
					error = ex.Message
				});
			}
		}
	}
}