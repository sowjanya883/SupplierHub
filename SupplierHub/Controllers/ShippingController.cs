using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.ShippingDto;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ShippingController : ControllerBase
	{
		private readonly IShippingService _service;

		public ShippingController(IShippingService service)
		{
			_service = service;
		}

		[HttpPost("shipments")]
		public async Task<IActionResult> CreateShipment([FromBody] ShipmentCreateDto dto)
		{
			var result = await _service.CreateShipmentAsync(dto);
			return Ok(result);
		}

		[HttpPost("asn")]
		public async Task<IActionResult> CreateAsn([FromBody] AsnCreateDto dto)
		{
			var result = await _service.CreateAsnAsync(dto);
			return Ok(result);
		}

		[HttpPost("asn/items")]
		public async Task<IActionResult> AddAsnItem([FromBody] AsnItemCreateDto dto)
		{
			var result = await _service.AddAsnItemAsync(dto);
			return Ok(result);
		}

		[HttpPost("slots")]
		public async Task<IActionResult> CreateSlot([FromBody] DeliverySlotCreateDto dto)
		{
			var result = await _service.CreateDeliverySlotAsync(dto);
			return Ok(result);
		}

		[HttpGet("slots/{siteId}")]
		public async Task<IActionResult> GetAvailableSlots(long siteId)
		{
			var result = await _service.GetAvailableSlotsAsync(siteId);
			return Ok(result);
		}
	}
}