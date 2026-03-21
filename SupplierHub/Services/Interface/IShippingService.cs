using SupplierHub.DTOs.ShippingDto;

namespace SupplierHub.Services.Interface
{
	public interface IShippingService
	{
		// Shipment
		Task<ShipmentReadDto> CreateShipmentAsync(ShipmentCreateDto dto);

		// ASN (Advanced Shipping Notice)
		Task<AsnReadDto> CreateAsnAsync(AsnCreateDto dto);

		// ASN Items
		Task<AsnItemReadDto> AddAsnItemAsync(AsnItemCreateDto dto);

		// Delivery Slots
		Task<DeliverySlotReadDto> CreateDeliverySlotAsync(DeliverySlotCreateDto dto);
		Task<List<DeliverySlotReadDto>> GetAvailableSlotsAsync(long siteId);
	}
}