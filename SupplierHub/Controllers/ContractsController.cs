using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.ContractDTO;
using Microsoft.AspNetCore.Authorization;
using SupplierHub.Constants;

namespace SupplierHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ContractsController : ControllerBase
	{
		private readonly IContractService _service;

		public ContractsController(IContractService service)
		{
			_service = service;
		}

		/// <summary>
		/// Create contract
		/// </summary>
		[HttpPost]
		[ProducesResponseType(typeof(ContractGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Create(
			[FromBody] ContractCreateDto dto,
			CancellationToken ct)
		{
			try
			{
				var created = await _service.CreateAsync(dto, ct);
				return Ok(new { message = "Contract created successfully.", data = created });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating contract.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get contract by ID
		/// </summary>
		[HttpGet("{contractId:long}")]
		[Authorize(Roles =
			nameof(RoleType.Admin) + "," +
			nameof(RoleType.CategoryManager) + "," +
			nameof(RoleType.SupplierUser))]
		[ProducesResponseType(typeof(ContractGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(long contractId, CancellationToken ct)
		{
			try
			{
				var contract = await _service.GetByIdAsync(contractId, ct);
				if (contract == null)
					return NotFound(new { message = $"Contract with ID {contractId} not found." });

				return Ok(new { message = "Contract retrieved successfully.", data = contract });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving contract.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Get all contracts
		/// </summary>
		[HttpGet]
		[Authorize(Roles =
			nameof(RoleType.Admin) + "," +
			nameof(RoleType.CategoryManager) + "," +
			nameof(RoleType.SupplierUser))]
		[ProducesResponseType(typeof(IEnumerable<ContractGetAllDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			try
			{
				var contracts = await _service.GetAllAsync(ct);
				return Ok(new { message = "Contracts retrieved successfully.", data = contracts });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving contracts.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Update contract
		/// </summary>
		[HttpPut("{contractId:long}")]
		[Authorize(Roles = nameof(RoleType.Admin) + "," + nameof(RoleType.CategoryManager))]
		[ProducesResponseType(typeof(ContractGetByIdDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(
			long contractId,
			[FromBody] ContractUpdateDto dto,
			CancellationToken ct)
		{
			try
			{
				if (dto.ContractID != contractId)
					return BadRequest(new { message = "Contract ID mismatch." });

				var updated = await _service.UpdateAsync(dto, ct);
				if (updated == null)
					return NotFound(new { message = $"Contract with ID {contractId} not found." });

				return Ok(new { message = "Contract updated successfully.", data = updated });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while updating contract.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Soft delete contract
		/// </summary>
		[HttpDelete]
		[Authorize(Roles = nameof(RoleType.Admin))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Delete(
			[FromBody] ContractDeleteDto dto,
			CancellationToken ct)
		{
			try
			{
				var deleted = await _service.DeleteAsync(dto, ct);
				if (!deleted)
					return NotFound(new { message = "No matching contract found to delete." });

				return Ok(new { message = "Contract deleted successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while deleting contract.",
					error = ex.Message
				});
			}
		}
	}
}