using SupplierHub.DTOs.SupplierKpiDTO;
using SupplierHub.DTOs.ScorecardDTO;

namespace SupplierHub.Services.Interface
{
    public interface IPerformanceService
    {
        // KPI Logic
        Task<SupplierKpiReadDto> CreateKpiAsync(SupplierKpiCreateDto dto);
        Task<List<SupplierKpiReadDto>> GetAllKpisAsync();
        Task<SupplierKpiReadDto?> GetKpiByIdAsync(long id);
        Task<List<SupplierKpiReadDto>> GetKpisBySupplierAsync(long supplierId);
        Task<SupplierKpiReadDto?> UpdateKpiAsync(long id, SupplierKpiUpdateDto dto);
        Task<bool> DeleteKpiAsync(long id);

        // Scorecard Logic
        Task<ScorecardReadDto> CreateScorecardAsync(ScorecardCreateDto dto);
        Task<List<ScorecardReadDto>> GetAllScorecardsAsync();
        Task<ScorecardReadDto?> GetScorecardByIdAsync(long id);
        Task<List<ScorecardReadDto>> GetScorecardsBySupplierAsync(long supplierId);
        Task<ScorecardReadDto?> UpdateScorecardAsync(long id, ScorecardUpdateDto dto);
        Task<bool> DeleteScorecardAsync(long id);
    }
}