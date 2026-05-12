using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class ShippingRepository : IShippingRepository
	{
		private readonly AppDbContext _context;

		public ShippingRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Shipment> AddShipmentAsync(Shipment shipment)
		{
			await _context.Shipments.AddAsync(shipment);
			await _context.SaveChangesAsync();
			return shipment;
		}

		public Task<List<Shipment>> GetAllShipmentsAsync() =>
			_context.Shipments.Where(s => !s.IsDeleted).OrderByDescending(s => s.CreatedOn).ToListAsync();

		public Task<Shipment?> GetShipmentByIdAsync(long shipmentId) =>
			_context.Shipments.FirstOrDefaultAsync(s => s.ShipmentID == shipmentId);

		public async Task<Shipment?> UpdateShipmentAsync(Shipment shipment)
		{
			_context.Shipments.Update(shipment);
			await _context.SaveChangesAsync();
			return shipment;
		}

		public async Task<Asn> AddAsnAsync(Asn asn)
		{
			await _context.Asns.AddAsync(asn);
			await _context.SaveChangesAsync();
			return asn;
		}

		public Task<List<Asn>> GetAllAsnsAsync() =>
			_context.Asns.Where(a => !a.IsDeleted).OrderByDescending(a => a.CreatedOn).ToListAsync();

		public Task<Asn?> GetAsnByIdAsync(long asnId) =>
			_context.Asns.FirstOrDefaultAsync(a => a.AsnID == asnId);

		public async Task<Asn?> UpdateAsnAsync(Asn asn)
		{
			_context.Asns.Update(asn);
			await _context.SaveChangesAsync();
			return asn;
		}

		public async Task<AsnItem> AddAsnItemAsync(AsnItem item)
		{
			await _context.AsnItems.AddAsync(item);
			await _context.SaveChangesAsync();
			return item;
		}

		public Task<List<AsnItem>> GetAsnItemsByAsnAsync(long asnId) =>
			_context.AsnItems.Where(i => i.AsnID == asnId && !i.IsDeleted).ToListAsync();

		public async Task<DeliverySlot> AddSlotAsync(DeliverySlot slot)
		{
			await _context.DeliverySlots.AddAsync(slot);
			await _context.SaveChangesAsync();
			return slot;
		}

		public async Task<List<DeliverySlot>> GetSlotsBySiteAsync(long siteId) =>
			await _context.DeliverySlots.Where(s => s.SiteID == siteId && !s.IsDeleted).ToListAsync();

		public Task<DeliverySlot?> GetSlotByIdAsync(long slotId) =>
			_context.DeliverySlots.FirstOrDefaultAsync(s => s.SlotID == slotId);

		public async Task<DeliverySlot?> UpdateSlotAsync(DeliverySlot slot)
		{
			_context.DeliverySlots.Update(slot);
			await _context.SaveChangesAsync();
			return slot;
		}
	}
}
