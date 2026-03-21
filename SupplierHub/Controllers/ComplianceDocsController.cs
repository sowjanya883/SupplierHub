using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.ComplianceDocDTO;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ComplianceDocsController : ControllerBase
	{
		private readonly IComplianceDocService _service;

		public ComplianceDocsController(IComplianceDocService service)
		{
			_service = service;
		}

		[HttpPost]
		[ProducesResponseType(typeof(ComplianceDocGetByIdDto), StatusCodes.Status200OK)]
		public async Task<IActionResult> Create(
			[FromBody] ComplianceDocCreateDto dto,
			CancellationToken ct)
		{
			try
			{
				var result = await _service.CreateAsync(dto, ct);
				return Ok(new { message = "Compliance document created successfully.", data = result });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error creating compliance document.", error = ex.Message });
			}
		}

		[HttpGet("{docId:long}")]
		public async Task<IActionResult> GetById(long docId, CancellationToken ct)
		{
			var result = await _service.GetByIdAsync(docId, ct);
			if (result == null)
				return NotFound(new { message = "Compliance document not found." });

			return Ok(new { message = "Compliance document retrieved successfully.", data = result });
		}

		[HttpGet]
		public async Task<IActionResult> GetAll(CancellationToken ct)
		{
			var result = await _service.GetAllAsync(ct);
			return Ok(new { message = "Compliance documents retrieved successfully.", data = result });
		}

		[HttpPut("{docId:long}")]
		public async Task<IActionResult> Update(
			long docId,
			[FromBody] ComplianceDocUpdateDto dto,
			CancellationToken ct)
		{
			if (dto.DocID != docId)
				return BadRequest(new { message = "Document ID mismatch." });

			var result = await _service.UpdateAsync(dto, ct);
			if (result == null)
				return NotFound(new { message = "Compliance document not found." });

			return Ok(new { message = "Compliance document updated successfully.", data = result });
		}

		[HttpDelete]
		public async Task<IActionResult> Delete(
			[FromBody] ComplianceDocDeleteDto dto,
			CancellationToken ct)
		{
			var deleted = await _service.DeleteAsync(dto, ct);
			if (!deleted)
				return NotFound(new { message = "Compliance document not found." });

			return Ok(new { message = "Compliance document deleted successfully." });
		}
	}
}