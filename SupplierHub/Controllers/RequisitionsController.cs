using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.RequisitionDto;
using SupplierHub.DTOs.ApprovalDto;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RequisitionsController : ControllerBase
	{
		private readonly IRequisitionService _service;

		public RequisitionsController(IRequisitionService service)
		{
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> CreateRequisition([FromBody] RequisitionCreateDto dto)
		{
			var result = await _service.CreateRequisitionAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = result.PrID }, result);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _service.GetAllRequisitionsAsync();
			return Ok(result);
		}

		[HttpGet("{id:long}")]
		public async Task<IActionResult> GetById(long id)
		{
			var result = await _service.GetRequisitionByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}

		[HttpGet("{prId:long}/lines")]
		public async Task<IActionResult> GetLines(long prId)
		{
			var result = await _service.GetLinesByPrIdAsync(prId);
			return Ok(result);
		}

		[HttpPost("lines")]
		public async Task<IActionResult> AddLineItem([FromBody] PrLineCreateDto dto)
		{
			var result = await _service.AddPrLineAsync(dto);
			return Ok(result);
		}

		[HttpGet("{prId:long}/approvals")]
		public async Task<IActionResult> GetApprovals(long prId)
		{
			var result = await _service.GetApprovalsByPrIdAsync(prId);
			return Ok(result);
		}

		[HttpPost("approvals")]
		public async Task<IActionResult> CreateApprovalStep([FromBody] ApprovalStepCreateDto dto)
		{
			var result = await _service.CreateApprovalStepAsync(dto);
			return Ok(result);
		}

		[HttpPut("approvals/{stepId}")]
		public async Task<IActionResult> UpdateApproval(long stepId, [FromBody] ApprovalStepUpdateDto dto)
		{
			var result = await _service.UpdateApprovalDecisionAsync(stepId, dto);
			return result == null ? NotFound() : Ok(result);
		}
	}
}