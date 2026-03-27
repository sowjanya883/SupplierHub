using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.PoRevisionDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin, Buyer, SupplierUser")]
	public class PoRevisionsController : ControllerBase
	{
		private readonly IPoRevisionService _service;

		public PoRevisionsController(IPoRevisionService service)
		{
			_service = service;
		}

		[HttpGet("po/{poId}")]
		public async Task<ActionResult<IEnumerable<PoRevisionResponseDto>>> GetByPoId(long poId)
		{
			try
			{
				var revisions = await _service.GetAllByPoIdAsync(poId);
				return Ok(revisions);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<PoRevisionResponseDto>> GetById(long id)
		{
			try
			{
				var revision = await _service.GetByIdAsync(id);

				if (revision == null)
					return NotFound();

				return Ok(revision);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPost]
		public async Task<ActionResult<PoRevisionResponseDto>> Create([FromBody] PoRevisionCreateDto createDto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				if (createDto == null)
					return BadRequest("PO Revision data is null.");

				var result = await _service.CreateAsync(createDto);

				// Note: Maintained your exact property casing 'PorevID' here
				return CreatedAtAction(nameof(GetById), new { id = result.PorevID }, result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}