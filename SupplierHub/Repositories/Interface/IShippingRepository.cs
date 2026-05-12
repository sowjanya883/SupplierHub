using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IShippingRepository
	{
		Task<Shipment> AddShipmentAsync(Shipment shipment);
		Task<List<Shipment>> GetAllShipmentsAsync();
		Task<Shipment?> GetShipmentByIdAsync(long shipmentId);
		Task<Shipment?> UpdateShipmentAsync(Shipment shipment);

		Task<Asn> AddAsnAsync(Asn asn);
		Task<List<Asn>> GetAllAsnsAsync();
		Task<Asn?> GetAsnByIdAsync(long asnId);
		Task<Asn?> UpdateAsnAsync(Asn asn);

		Task<AsnItem> AddAsnItemAsync(AsnItem item);
		Task<List<AsnItem>> GetAsnItemsByAsnAsync(long asnId);

		Task<DeliverySlot> AddSlotAsync(DeliverySlot slot);
		Task<List<DeliverySlot>> GetSlotsBySiteAsync(long siteId);
		Task<DeliverySlot?> GetSlotByIdAsync(long slotId);
		Task<DeliverySlot?> UpdateSlotAsync(DeliverySlot slot);
	}
}