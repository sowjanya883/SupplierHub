using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.BidDTO;
using SupplierHub.DTOs.BidLineDTO;
using SupplierHub.DTOs.AwardDTO;
using SupplierHub.DTOs.RfxInviteDTO;
using SupplierHub.DTOs.RFxLineDTO;
using SupplierHub.DTOs.RfxEventDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	/// <summary>
	/// Controller for managing RFx (Request for eXchange) operations including events, lines, invites, bids, and awards.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class RfxController : ControllerBase
	{
		private readonly IRfxService _service;

		/// <summary>
		/// Initializes a new instance of the RfxController class.
		/// </summary>
		/// <param name="service">The RFx service instance</param>
		public RfxController(IRfxService service)
		{
			_service = service;
		}

		/// <summary>
		/// Retrieves all RFx events.
		/// </summary>
		/// <returns>List of RFx events</returns>
		[HttpGet("rfx")]
		public async Task<IActionResult> GetAllRfx() =>
			Ok(await _service.GetAllRfxAsync());

		/// <summary>
		/// Retrieves a specific RFx event by ID.
		/// </summary>
		/// <param name="id">The RFx event ID</param>
		/// <returns>RFx event details if found; 404 if not found</returns>
		[HttpGet("rfx/{id:long}")]
		public async Task<IActionResult> GetRfx(long id)
		{
			var result = await _service.GetRfxByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Creates a new RFx event.
		/// </summary>
		/// <param name="dto">RFx event creation DTO</param>
		/// <returns>Created RFx event details</returns>
		[HttpPost("rfx")]
		public async Task<IActionResult> CreateRfx(RFxEventCreateDto dto) =>
			Ok(await _service.CreateRfxAsync(dto));

		/// <summary>
		/// Updates an existing RFx event.
		/// </summary>
		/// <param name="dto">RFx event update DTO</param>
		/// <returns>Updated RFx event details if found; 404 if not found</returns>
		[HttpPut("rfx")]
		public async Task<IActionResult> UpdateRfx(RFxEventUpdateDto dto)
		{
			var result = await _service.UpdateRfxAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves all RFx lines for a specific RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of RFx lines</returns>
		[HttpGet("rfx/{rfxId:long}/lines")]
		public async Task<IActionResult> GetRfxLines(long rfxId) =>
			Ok(await _service.GetLinesByRfxAsync(rfxId));

		/// <summary>
		/// Adds a new RFx line to an RFx event.
		/// </summary>
		/// <param name="dto">RFx line creation DTO</param>
		/// <returns>Created RFx line details</returns>
		[HttpPost("rfx-lines")]
		public async Task<IActionResult> AddRfxLine(RfxLineCreateDto dto) =>
			Ok(await _service.AddRfxLineAsync(dto));

		/// <summary>
		/// Updates an existing RFx line.
		/// </summary>
		/// <param name="dto">RFx line update DTO</param>
		/// <returns>Updated RFx line details if found; 404 if not found</returns>
		[HttpPut("rfx-lines")]
		public async Task<IActionResult> UpdateRfxLine(RfxLineUpdateDto dto)
		{
			var result = await _service.UpdateRfxLineAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves all RFx invites for a specific RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of RFx invites</returns>
		[HttpGet("rfx/{rfxId:long}/invites")]
		public async Task<IActionResult> GetRfxInvites(long rfxId) =>
			Ok(await _service.GetInvitesByRfxAsync(rfxId));

		/// <summary>
		/// Creates a new RFx invite for a supplier.
		/// </summary>
		/// <param name="dto">RFx invite creation DTO</param>
		/// <returns>Created RFx invite details</returns>
		[HttpPost("invites")]
		public async Task<IActionResult> AddInvite(RfxInviteCreateDto dto) =>
			Ok(await _service.AddInviteAsync(dto));

		/// <summary>
		/// Updates an existing RFx invite status.
		/// </summary>
		/// <param name="dto">RFx invite update DTO</param>
		/// <returns>Updated RFx invite details if found; 404 if not found</returns>
		[HttpPut("invites")]
		public async Task<IActionResult> UpdateInvite(RfxInviteUpdateDto dto)
		{
			var result = await _service.UpdateInviteAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves all bids for a specific RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of bids</returns>
		[HttpGet("rfx/{rfxId:long}/bids")]
		public async Task<IActionResult> GetBids(long rfxId) =>
			Ok(await _service.GetBidsByRfxAsync(rfxId));

		/// <summary>
		/// Retrieves a specific bid by ID.
		/// </summary>
		/// <param name="bidId">The bid ID</param>
		/// <returns>Bid details if found; 404 if not found</returns>
		[HttpGet("bids/{bidId:long}")]
		public async Task<IActionResult> GetBid(long bidId)
		{
			var result = await _service.GetBidByIdAsync(bidId);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Creates a new bid for an RFx event.
		/// </summary>
		/// <param name="dto">Bid creation DTO</param>
		/// <returns>Created bid details</returns>
		[HttpPost("bids")]
		public async Task<IActionResult> AddBid(BidCreateDto dto) =>
			Ok(await _service.AddBidAsync(dto));

		/// <summary>
		/// Updates an existing bid.
		/// </summary>
		/// <param name="dto">Bid update DTO</param>
		/// <returns>Updated bid details if found; 404 if not found</returns>
		[HttpPut("bids")]
		public async Task<IActionResult> UpdateBid(BidUpdateDto dto)
		{
			var result = await _service.UpdateBidAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves all bid lines for a specific bid.
		/// </summary>
		/// <param name="bidId">The bid ID</param>
		/// <returns>List of bid lines</returns>
		[HttpGet("bids/{bidId:long}/lines")]
		public async Task<IActionResult> GetBidLines(long bidId) =>
			Ok(await _service.GetBidLinesByBidAsync(bidId));

		/// <summary>
		/// Adds a new bid line to a bid.
		/// </summary>
		/// <param name="dto">Bid line creation DTO</param>
		/// <returns>Created bid line details</returns>
		[HttpPost("bid-lines")]
		public async Task<IActionResult> AddBidLine(BidLineCreateDto dto) =>
			Ok(await _service.AddBidLineAsync(dto));

		/// <summary>
		/// Updates an existing bid line.
		/// </summary>
		/// <param name="dto">Bid line update DTO</param>
		/// <returns>Updated bid line details if found; 404 if not found</returns>
		[HttpPut("bid-lines")]
		public async Task<IActionResult> UpdateBidLine(BidLineUpdateDto dto)
		{
			var result = await _service.UpdateBidLineAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves all awards for a specific RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of awards</returns>
		[HttpGet("rfx/{rfxId:long}/awards")]
		public async Task<IActionResult> GetAwards(long rfxId) =>
			Ok(await _service.GetAwardsByRfxAsync(rfxId));

		/// <summary>
		/// Creates a new award for an RFx event.
		/// </summary>
		/// <param name="dto">Award creation DTO</param>
		/// <returns>Created award details</returns>
		[HttpPost("awards")]
		public async Task<IActionResult> AddAward(AwardCreateDto dto) =>
			Ok(await _service.AddAwardAsync(dto));

		/// <summary>
		/// Updates an existing award.
		/// </summary>
		/// <param name="dto">Award update DTO</param>
		/// <returns>Updated award details if found; 404 if not found</returns>
		[HttpPut("awards")]
		public async Task<IActionResult> UpdateAward(AwardUpdateDto dto)
		{
			var result = await _service.UpdateAwardAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}
	}
}
