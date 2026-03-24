using AutoMapper;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.SupplierKpiDTO;
using SupplierHub.DTOs.ScorecardDTO;

namespace SupplierHub.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly IPerformanceRepository _repo;
        private readonly IMapper _mapper;

        public PerformanceService(IPerformanceRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // =======================
        // --- KPI Methods ---
        // =======================
        public async Task<SupplierKpiReadDto> CreateKpiAsync(SupplierKpiCreateDto dto)
        {
            var entity = _mapper.Map<SupplierKpi>(dto);
            var saved = await _repo.AddKpiAsync(entity);
            return _mapper.Map<SupplierKpiReadDto>(saved);
        }

        public async Task<List<SupplierKpiReadDto>> GetAllKpisAsync()
        {
            var entities = await _repo.GetAllKpisAsync();
            return _mapper.Map<List<SupplierKpiReadDto>>(entities);
        }

        public async Task<SupplierKpiReadDto?> GetKpiByIdAsync(long id)
        {
            var entity = await _repo.GetKpiByIdAsync(id);
            return entity == null ? null : _mapper.Map<SupplierKpiReadDto>(entity);
        }

        public async Task<List<SupplierKpiReadDto>> GetKpisBySupplierAsync(long supplierId)
        {
            var entities = await _repo.GetKpisBySupplierIdAsync(supplierId);
            return _mapper.Map<List<SupplierKpiReadDto>>(entities);
        }

        public async Task<SupplierKpiReadDto?> UpdateKpiAsync(long id, SupplierKpiUpdateDto dto)
        {
            var existing = await _repo.GetKpiByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateKpiAsync(existing);
            return _mapper.Map<SupplierKpiReadDto>(updated);
        }

        public async Task<bool> DeleteKpiAsync(long id) =>
            await _repo.DeleteKpiAsync(id);


        // =======================
        // --- Scorecard Methods ---
        // =======================
        public async Task<ScorecardReadDto> CreateScorecardAsync(ScorecardCreateDto dto)
        {
            var entity = _mapper.Map<Scorecard>(dto);
            var saved = await _repo.AddScorecardAsync(entity);
            return _mapper.Map<ScorecardReadDto>(saved);
        }

        public async Task<List<ScorecardReadDto>> GetAllScorecardsAsync()
        {
            var entities = await _repo.GetAllScorecardsAsync();
            return _mapper.Map<List<ScorecardReadDto>>(entities);
        }

        public async Task<ScorecardReadDto?> GetScorecardByIdAsync(long id)
        {
            var entity = await _repo.GetScorecardByIdAsync(id);
            return entity == null ? null : _mapper.Map<ScorecardReadDto>(entity);
        }

        public async Task<List<ScorecardReadDto>> GetScorecardsBySupplierAsync(long supplierId)
        {
            var entities = await _repo.GetScorecardsBySupplierIdAsync(supplierId);
            return _mapper.Map<List<ScorecardReadDto>>(entities);
        }

        public async Task<ScorecardReadDto?> UpdateScorecardAsync(long id, ScorecardUpdateDto dto)
        {
            var existing = await _repo.GetScorecardByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateScorecardAsync(existing);
            return _mapper.Map<ScorecardReadDto>(updated);
        }

        public async Task<bool> DeleteScorecardAsync(long id) =>
            await _repo.DeleteScorecardAsync(id);
    }
}