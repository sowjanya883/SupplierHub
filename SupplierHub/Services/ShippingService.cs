using AutoMapper;
using SupplierHub.DTOs.ShippingDto;
using SupplierHub.Models;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface; // Ensure this is here

namespace SupplierHub.Services
{
	public class ShippingService : IShippingService
	{
		private readonly IShippingRepository _repo;
		private readonly IMapper _mapper;

		public ShippingService(IShippingRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<ShipmentReadDto> CreateShipmentAsync(ShipmentCreateDto dto)
		{
			var entity = _mapper.Map<Shipment>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow; // Match your model's [Required] attribute
			entity.IsDeleted = false;
			var saved = await _repo.AddShipmentAsync(entity);
			return _mapper.Map<ShipmentReadDto>(saved);
		}

		public async Task<AsnReadDto> CreateAsnAsync(AsnCreateDto dto)
		{
			var entity = _mapper.Map<Asn>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;
			var saved = await _repo.AddAsnAsync(entity);
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
			return _mapper.Map<DeliverySlotReadDto>(saved);
		}

		// --- ADD THIS TO FIX THE INTERFACE ERROR ---
		public async Task<List<DeliverySlotReadDto>> GetAvailableSlotsAsync(long siteId)
		{
			var slots = await _repo.GetSlotsBySiteAsync(siteId);
			// Filters for only 'AVAILABLE' status if needed
			var availableSlots = slots.Where(s => s.Status == "AVAILABLE").ToList();
			return _mapper.Map<List<DeliverySlotReadDto>>(availableSlots);
		}
	}
}