using SupplierHub.DTOs.ShippingDto;

namespace SupplierHub.Services.Interface
{
	public interface IShippingService
	{
		// Shipment
		Task<ShipmentReadDto> CreateShipmentAsync(ShipmentCreateDto dto);
		Task<List<ShipmentReadDto>> GetAllShipmentsAsync();
		Task<ShipmentReadDto?> GetShipmentByIdAsync(long shipmentId);
		Task<ShipmentReadDto?> UpdateShipmentAsync(long shipmentId, ShipmentUpdateDto dto);

		// ASN (Advanced Shipping Notice)
		Task<AsnReadDto> CreateAsnAsync(AsnCreateDto dto);
		Task<List<AsnReadDto>> GetAllAsnsAsync();
		Task<AsnReadDto?> GetAsnByIdAsync(long asnId);
		Task<AsnReadDto?> UpdateAsnAsync(long asnId, AsnUpdateDto dto);

		// ASN Items
		Task<AsnItemReadDto> AddAsnItemAsync(AsnItemCreateDto dto);
		Task<List<AsnItemReadDto>> GetAsnItemsByAsnAsync(long asnId);

		// Delivery Slots
		Task<DeliverySlotReadDto> CreateDeliverySlotAsync(DeliverySlotCreateDto dto);
		Task<List<DeliverySlotReadDto>> GetAvailableSlotsAsync(long siteId);
		Task<DeliverySlotReadDto?> UpdateSlotStatusAsync(long slotId, string newStatus);
	}
}