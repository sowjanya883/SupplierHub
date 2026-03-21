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

		public async Task<Asn> AddAsnAsync(Asn asn)
		{
			await _context.Asns.AddAsync(asn);
			await _context.SaveChangesAsync();
			return asn;
		}

		public async Task<AsnItem> AddAsnItemAsync(AsnItem item)
		{
			await _context.AsnItems.AddAsync(item);
			await _context.SaveChangesAsync();
			return item;
		}

		public async Task<DeliverySlot> AddSlotAsync(DeliverySlot slot)
		{
			await _context.DeliverySlots.AddAsync(slot);
			await _context.SaveChangesAsync();
			return slot;
		}

		public async Task<List<DeliverySlot>> GetSlotsBySiteAsync(long siteId) =>
			await _context.DeliverySlots.Where(s => s.SiteID == siteId && !s.IsDeleted).ToListAsync();
	}
}
