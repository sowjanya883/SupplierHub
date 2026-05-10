using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
			try
			{
				var result = await _service.CreateShipmentAsync(dto);
				return Ok(result);
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(new { message = "Could not create shipment — check that PO and Supplier IDs exist.", detail = ex.InnerException?.Message ?? ex.Message });
			}
			catch (System.Exception ex)
			{
				return BadRequest(new { message = "Could not create shipment.", detail = ex.Message });
			}
		}

		[HttpPost("asn")]
		public async Task<IActionResult> CreateAsn([FromBody] AsnCreateDto dto)
		{
			try
			{
				var result = await _service.CreateAsnAsync(dto);
				return Ok(result);
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(new { message = "Could not create ASN — check that the Shipment ID exists.", detail = ex.InnerException?.Message ?? ex.Message });
			}
			catch (System.Exception ex)
			{
				return BadRequest(new { message = "Could not create ASN.", detail = ex.Message });
			}
		}

		[HttpPost("asn/items")]
		public async Task<IActionResult> AddAsnItem([FromBody] AsnItemCreateDto dto)
		{
			try
			{
				var result = await _service.AddAsnItemAsync(dto);
				return Ok(result);
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(new { message = "Could not add ASN item — check that ASN ID and PO Line ID exist.", detail = ex.InnerException?.Message ?? ex.Message });
			}
			catch (System.Exception ex)
			{
				return BadRequest(new { message = "Could not add ASN item.", detail = ex.Message });
			}
		}

		[HttpPost("slots")]
		public async Task<IActionResult> CreateSlot([FromBody] DeliverySlotCreateDto dto)
		{
			try
			{
				var result = await _service.CreateDeliverySlotAsync(dto);
				return Ok(result);
			}
			catch (System.Exception ex)
			{
				return BadRequest(new { message = "Could not create delivery slot.", detail = ex.Message });
			}
		}

		[HttpGet("slots/{siteId}")]
		public async Task<IActionResult> GetAvailableSlots(long siteId)
		{
			var result = await _service.GetAvailableSlotsAsync(siteId);
			return Ok(result);
		}
	}
}