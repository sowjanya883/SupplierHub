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

		[HttpPut("slots/{slotId:long}/status")]
		public async Task<IActionResult> UpdateSlotStatus(long slotId, [FromBody] DeliverySlotStatusDto dto)
		{
			try
			{
				var updated = await _service.UpdateSlotStatusAsync(slotId, dto.Status);
				if (updated == null) return NotFound(new { message = $"Delivery slot {slotId} not found." });
				return Ok(new { message = "Slot status updated.", data = updated });
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(new { message = "Could not update slot.", detail = ex.InnerException?.Message ?? ex.Message });
			}
			catch (System.Exception ex)
			{
				return BadRequest(new { message = "Could not update slot.", detail = ex.Message });
			}
		}

		[HttpGet("shipments")]
		public async Task<IActionResult> GetAllShipments()
		{
			var result = await _service.GetAllShipmentsAsync();
			return Ok(new { message = "Shipments retrieved.", data = result });
		}

		[HttpGet("shipments/{id:long}")]
		public async Task<IActionResult> GetShipmentById(long id)
		{
			var result = await _service.GetShipmentByIdAsync(id);
			if (result == null) return NotFound(new { message = $"Shipment {id} not found." });
			return Ok(new { data = result });
		}

		[HttpPut("shipments/{id:long}")]
		public async Task<IActionResult> UpdateShipment(long id, [FromBody] ShipmentUpdateDto dto)
		{
			try
			{
				var updated = await _service.UpdateShipmentAsync(id, dto);
				if (updated == null) return NotFound(new { message = $"Shipment {id} not found." });
				return Ok(new { message = "Shipment updated.", data = updated });
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(new { message = "Could not update shipment.", detail = ex.InnerException?.Message ?? ex.Message });
			}
			catch (System.Exception ex)
			{
				return BadRequest(new { message = "Could not update shipment.", detail = ex.Message });
			}
		}

		[HttpGet("asn")]
		public async Task<IActionResult> GetAllAsns()
		{
			var result = await _service.GetAllAsnsAsync();
			return Ok(new { message = "ASNs retrieved.", data = result });
		}

		[HttpGet("asn/{id:long}")]
		public async Task<IActionResult> GetAsnById(long id)
		{
			var result = await _service.GetAsnByIdAsync(id);
			if (result == null) return NotFound(new { message = $"ASN {id} not found." });
			return Ok(new { data = result });
		}

		[HttpPut("asn/{id:long}")]
		public async Task<IActionResult> UpdateAsn(long id, [FromBody] AsnUpdateDto dto)
		{
			try
			{
				var updated = await _service.UpdateAsnAsync(id, dto);
				if (updated == null) return NotFound(new { message = $"ASN {id} not found." });
				return Ok(new { message = "ASN updated.", data = updated });
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(new { message = "Could not update ASN.", detail = ex.InnerException?.Message ?? ex.Message });
			}
			catch (System.Exception ex)
			{
				return BadRequest(new { message = "Could not update ASN.", detail = ex.Message });
			}
		}

		[HttpGet("asn/{asnId:long}/items")]
		public async Task<IActionResult> GetAsnItems(long asnId)
		{
			var result = await _service.GetAsnItemsByAsnAsync(asnId);
			return Ok(new { data = result });
		}
	}
}