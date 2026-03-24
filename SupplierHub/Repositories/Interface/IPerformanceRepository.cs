using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
    public interface IPerformanceRepository
    {
        // --- KPI ---
        Task<SupplierKpi> AddKpiAsync(SupplierKpi kpi);
        Task<List<SupplierKpi>> GetAllKpisAsync(); // New: Get All
        Task<SupplierKpi?> GetKpiByIdAsync(long id); // New: Get By Id
        Task<List<SupplierKpi>> GetKpisBySupplierIdAsync(long supplierId);
        Task<SupplierKpi?> UpdateKpiAsync(SupplierKpi kpi);
        Task<bool> DeleteKpiAsync(long id); // New: Delete

        // --- Scorecard ---
        Task<Scorecard> AddScorecardAsync(Scorecard scorecard);
        Task<List<Scorecard>> GetAllScorecardsAsync(); // New: Get All
        Task<Scorecard?> GetScorecardByIdAsync(long id); // New: Get By Id
        Task<List<Scorecard>> GetScorecardsBySupplierIdAsync(long supplierId);
        Task<Scorecard?> UpdateScorecardAsync(Scorecard scorecard);
        Task<bool> DeleteScorecardAsync(long id); // New: Delete
    }
}