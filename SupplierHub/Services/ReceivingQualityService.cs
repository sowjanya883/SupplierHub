using AutoMapper;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.GrnRefDTO;
using SupplierHub.DTOs.GrnItemRefDTO;
using SupplierHub.DTOs.NcrDTO;
using SupplierHub.DTOs.InspectionDTO;

namespace SupplierHub.Services
{
    public class ReceivingQualityService : IReceivingQualityService
    {
        private readonly IReceivingQualityRepository _repo;
        private readonly IMapper _mapper;

        public ReceivingQualityService(IReceivingQualityRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // =======================
        // --- GRN Header Logic ---
        // =======================
        public async Task<GrnReadDto> CreateGrnAsync(GrnCreateDto dto)
        {
            var entity = _mapper.Map<GrnRef>(dto);
            var saved = await _repo.AddGrnAsync(entity);
            return _mapper.Map<GrnReadDto>(saved);
        }

        public async Task<List<GrnReadDto>> GetAllGrnsAsync() =>
            _mapper.Map<List<GrnReadDto>>(await _repo.GetAllGrnsAsync());

        public async Task<GrnReadDto?> GetGrnByIdAsync(long id)
        {
            var entity = await _repo.GetGrnByIdAsync(id);
            return entity == null ? null : _mapper.Map<GrnReadDto>(entity);
        }

        public async Task<GrnReadDto?> UpdateGrnAsync(long id, GrnUpdateDto dto)
        {
            var existing = await _repo.GetGrnByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateGrnAsync(existing);
            return _mapper.Map<GrnReadDto>(updated);
        }

        public async Task<bool> DeleteGrnAsync(long id) => await _repo.DeleteGrnAsync(id);

        // =======================
        // --- GRN Item Logic ---
        // =======================
        public async Task<GrnItemReadDto> AddGrnItemAsync(GrnItemCreateDto dto)
        {
            var entity = _mapper.Map<GrnItemRef>(dto);
            var saved = await _repo.AddGrnItemAsync(entity);
            return _mapper.Map<GrnItemReadDto>(saved);
        }

        public async Task<List<GrnItemReadDto>> GetAllGrnItemsAsync() =>
            _mapper.Map<List<GrnItemReadDto>>(await _repo.GetAllGrnItemsAsync());

        public async Task<GrnItemReadDto?> GetGrnItemByIdAsync(long id)
        {
            var entity = await _repo.GetGrnItemByIdAsync(id);
            return entity == null ? null : _mapper.Map<GrnItemReadDto>(entity);
        }

        public async Task<List<GrnItemReadDto>> GetGrnItemsByGrnIdAsync(long grnId)
        {
            var items = await _repo.GetItemsByGrnIdAsync(grnId);
            return _mapper.Map<List<GrnItemReadDto>>(items);
        }

        public async Task<GrnItemReadDto?> UpdateGrnItemAsync(long id, GrnItemUpdateDto dto)
        {
            var existing = await _repo.GetGrnItemByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateGrnItemAsync(existing);
            return _mapper.Map<GrnItemReadDto>(updated);
        }

        public async Task<bool> DeleteGrnItemAsync(long id) => await _repo.DeleteGrnItemAsync(id);

        // =======================
        // --- Inspection Logic ---
        // =======================
        public async Task<InspectionReadDto> CreateInspectionAsync(InspectionCreateDto dto)
        {
            var entity = _mapper.Map<Inspection>(dto);
            var saved = await _repo.AddInspectionAsync(entity);
            return _mapper.Map<InspectionReadDto>(saved);
        }

        public async Task<List<InspectionReadDto>> GetAllInspectionsAsync() =>
            _mapper.Map<List<InspectionReadDto>>(await _repo.GetAllInspectionsAsync());

        public async Task<InspectionReadDto?> GetInspectionByIdAsync(long id)
        {
            var entity = await _repo.GetInspectionByIdAsync(id);
            return entity == null ? null : _mapper.Map<InspectionReadDto>(entity);
        }

        public async Task<List<InspectionReadDto>> GetInspectionsByItemIdAsync(long grnItemId) =>
            _mapper.Map<List<InspectionReadDto>>(await _repo.GetInspectionsByItemIdAsync(grnItemId));

        public async Task<InspectionReadDto?> UpdateInspectionAsync(long id, InspectionUpdateDto dto)
        {
            var existing = await _repo.GetInspectionByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateInspectionAsync(existing);
            return _mapper.Map<InspectionReadDto>(updated);
        }

        public async Task<bool> DeleteInspectionAsync(long id) => await _repo.DeleteInspectionAsync(id);

        // =======================
        // --- NCR Logic ---
        // =======================
        public async Task<NcrReadDto> CreateNcrAsync(NcrCreateDto dto)
        {
            var entity = _mapper.Map<Ncr>(dto);
            var saved = await _repo.AddNcrAsync(entity);
            return _mapper.Map<NcrReadDto>(saved);
        }

        public async Task<List<NcrReadDto>> GetAllNcrsAsync() =>
            _mapper.Map<List<NcrReadDto>>(await _repo.GetAllNcrsAsync());

        public async Task<NcrReadDto?> GetNcrByIdAsync(long id)
        {
            var entity = await _repo.GetNcrByIdAsync(id);
            return entity == null ? null : _mapper.Map<NcrReadDto>(entity);
        }

        public async Task<List<NcrReadDto>> GetNcrsByItemIdAsync(long grnItemId) =>
            _mapper.Map<List<NcrReadDto>>(await _repo.GetNcrsByItemIdAsync(grnItemId));

        public async Task<NcrReadDto?> UpdateNcrAsync(long id, NcrUpdateDto dto)
        {
            var existing = await _repo.GetNcrByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateNcrAsync(existing);
            return _mapper.Map<NcrReadDto>(updated);
        }

        public async Task<bool> DeleteNcrAsync(long id) => await _repo.DeleteNcrAsync(id);
    }
}