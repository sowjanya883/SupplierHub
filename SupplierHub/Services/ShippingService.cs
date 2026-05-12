using AutoMapper;
using SupplierHub.DTOs.ShippingDto;
using SupplierHub.Models;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Services
{
	public class ShippingService : IShippingService
	{
		private readonly IShippingRepository _repo;
		private readonly IMapper _mapper;
		private readonly INotificationService _notif;

		public ShippingService(IShippingRepository repo, IMapper mapper, INotificationService notif)
		{
			_repo = repo;
			_mapper = mapper;
			_notif = notif;
		}

		public async Task<ShipmentReadDto> CreateShipmentAsync(ShipmentCreateDto dto)
		{
			var entity = _mapper.Map<Shipment>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;
			var saved = await _repo.AddShipmentAsync(entity);

			var msg = $"Shipment #{saved.ShipmentID} dispatched against PO-{saved.PoID} via {saved.Carrier ?? "carrier"}{(string.IsNullOrEmpty(saved.TrackingNo) ? "" : $" (tracking {saved.TrackingNo})")}.";
			await _notif.SendToRoleAsync("Admin", msg, "Shipment", saved.ShipmentID);
			await _notif.SendToRoleAsync("Buyer", msg, "Shipment", saved.ShipmentID);
			await _notif.SendToRoleAsync("ReceivingUser", msg, "Shipment", saved.ShipmentID);
			await _notif.SendToRoleAsync("WarehouseManager", msg, "Shipment", saved.ShipmentID);

			return _mapper.Map<ShipmentReadDto>(saved);
		}

		public async Task<AsnReadDto> CreateAsnAsync(AsnCreateDto dto)
		{
			var entity = _mapper.Map<Asn>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;
			var saved = await _repo.AddAsnAsync(entity);

			var msg = $"ASN {saved.AsnNo} created for Shipment #{saved.ShipmentID}. Receiving team to prepare.";
			await _notif.SendToRoleAsync("Admin", msg, "Shipment", saved.AsnID);
			await _notif.SendToRoleAsync("ReceivingUser", msg, "Shipment", saved.AsnID);
			await _notif.SendToRoleAsync("WarehouseManager", msg, "Shipment", saved.AsnID);

			return _mapper.Map<AsnReadDto>(saved);
		}

		public async Task<AsnItemReadDto> AddAsnItemAsync(AsnItemCreateDto dto)
		{
			var entity = _mapper.Map<AsnItem>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;
			var saved = await _repo.AddAsnItemAsync(entity);
			return _mapper.Map<AsnItemReadDto>(saved);
		}

		public async Task<DeliverySlotReadDto> CreateDeliverySlotAsync(DeliverySlotCreateDto dto)
		{
			var entity = _mapper.Map<DeliverySlot>(dto);
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;
			var saved = await _repo.AddSlotAsync(entity);

			var msg = $"Delivery slot #{saved.SlotID} booked at Site {saved.SiteID} on {saved.Date:yyyy-MM-dd} ({saved.StartTime}–{saved.EndTime}).";
			await _notif.SendToRoleAsync("Admin", msg, "Shipment", saved.SlotID);
			await _notif.SendToRoleAsync("ReceivingUser", msg, "Shipment", saved.SlotID);
			await _notif.SendToRoleAsync("WarehouseManager", msg, "Shipment", saved.SlotID);

			return _mapper.Map<DeliverySlotReadDto>(saved);
		}

		public async Task<List<DeliverySlotReadDto>> GetAvailableSlotsAsync(long siteId)
		{
			// Return ALL non-deleted slots for this site so the lookup reflects what
			// the user actually created (regardless of status). Caller can filter
			// in the UI if they only want AVAILABLE rows.
			var slots = await _repo.GetSlotsBySiteAsync(siteId);
			return _mapper.Map<List<DeliverySlotReadDto>>(slots);
		}

		public async Task<List<ShipmentReadDto>> GetAllShipmentsAsync()
		{
			var entities = await _repo.GetAllShipmentsAsync();
			return _mapper.Map<List<ShipmentReadDto>>(entities);
		}

		public async Task<ShipmentReadDto?> GetShipmentByIdAsync(long shipmentId)
		{
			var entity = await _repo.GetShipmentByIdAsync(shipmentId);
			return entity == null ? null : _mapper.Map<ShipmentReadDto>(entity);
		}

		public async Task<ShipmentReadDto?> UpdateShipmentAsync(long shipmentId, ShipmentUpdateDto dto)
		{
			var existing = await _repo.GetShipmentByIdAsync(shipmentId);
			if (existing == null) return null;

			if (dto.ShipDate.HasValue) existing.ShipDate = dto.ShipDate.Value;
			if (!string.IsNullOrWhiteSpace(dto.Carrier)) existing.Carrier = dto.Carrier;
			if (!string.IsNullOrWhiteSpace(dto.TrackingNo)) existing.TrackingNo = dto.TrackingNo;
			if (!string.IsNullOrWhiteSpace(dto.Status)) existing.Status = dto.Status;
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateShipmentAsync(existing);
			if (updated == null) return null;

			var msg = $"Shipment #{updated.ShipmentID} status updated to '{updated.Status}'.";
			await _notif.SendToRoleAsync("Admin", msg, "Shipment", updated.ShipmentID);
			await _notif.SendToRoleAsync("Buyer", msg, "Shipment", updated.ShipmentID);
			await _notif.SendToRoleAsync("ReceivingUser", msg, "Shipment", updated.ShipmentID);

			return _mapper.Map<ShipmentReadDto>(updated);
		}

		public async Task<List<AsnReadDto>> GetAllAsnsAsync()
		{
			var entities = await _repo.GetAllAsnsAsync();
			return _mapper.Map<List<AsnReadDto>>(entities);
		}

		public async Task<AsnReadDto?> GetAsnByIdAsync(long asnId)
		{
			var entity = await _repo.GetAsnByIdAsync(asnId);
			return entity == null ? null : _mapper.Map<AsnReadDto>(entity);
		}

		public async Task<AsnReadDto?> UpdateAsnAsync(long asnId, AsnUpdateDto dto)
		{
			var existing = await _repo.GetAsnByIdAsync(asnId);
			if (existing == null) return null;

			if (!string.IsNullOrWhiteSpace(dto.AsnNo)) existing.AsnNo = dto.AsnNo;
			if (dto.CreatedDate.HasValue) existing.CreatedDate = dto.CreatedDate.Value;
			if (!string.IsNullOrWhiteSpace(dto.Status)) existing.Status = dto.Status;
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsnAsync(existing);
			if (updated == null) return null;

			var msg = $"ASN {updated.AsnNo} status updated to '{updated.Status}'.";
			await _notif.SendToRoleAsync("Admin", msg, "Shipment", updated.AsnID);
			await _notif.SendToRoleAsync("ReceivingUser", msg, "Shipment", updated.AsnID);

			return _mapper.Map<AsnReadDto>(updated);
		}

		public async Task<List<AsnItemReadDto>> GetAsnItemsByAsnAsync(long asnId)
		{
			var entities = await _repo.GetAsnItemsByAsnAsync(asnId);
			return _mapper.Map<List<AsnItemReadDto>>(entities);
		}

		public async Task<DeliverySlotReadDto?> UpdateSlotStatusAsync(long slotId, string newStatus)
		{
			var existing = await _repo.GetSlotByIdAsync(slotId);
			if (existing == null) return null;

			existing.Status = newStatus;
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateSlotAsync(existing);
			if (updated == null) return null;

			var msg = $"Delivery slot #{updated.SlotID} at Site {updated.SiteID} is now '{updated.Status}'.";
			await _notif.SendToRoleAsync("Admin", msg, "Shipment", updated.SlotID);
			await _notif.SendToRoleAsync("ReceivingUser", msg, "Shipment", updated.SlotID);
			await _notif.SendToRoleAsync("WarehouseManager", msg, "Shipment", updated.SlotID);

			return _mapper.Map<DeliverySlotReadDto>(updated);
		}
	}
}
