using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IShippingRepository
	{
		Task<Shipment> AddShipmentAsync(Shipment shipment);
		Task<Asn> AddAsnAsync(Asn asn);
		Task<AsnItem> AddAsnItemAsync(AsnItem item);
		Task<DeliverySlot> AddSlotAsync(DeliverySlot slot);
		Task<List<DeliverySlot>> GetSlotsBySiteAsync(long siteId);
	}
}