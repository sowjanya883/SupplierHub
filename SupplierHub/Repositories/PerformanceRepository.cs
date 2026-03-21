using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
    public class PerformanceRepository : IPerformanceRepository
    {
        private readonly AppDbContext _db;

        public PerformanceRepository(AppDbContext db)
        {
            _db = db;
        }

        // =======================
        // --- KPI Methods ---
        // =======================
        public async Task<SupplierKpi> AddKpiAsync(SupplierKpi kpi)
        {
            _db.SupplierKpis.Add(kpi);
            await _db.SaveChangesAsync();
            return kpi;
        }

        public Task<List<SupplierKpi>> GetAllKpisAsync() =>
            _db.SupplierKpis.Where(x => !x.IsDeleted).ToListAsync();

        public Task<SupplierKpi?> GetKpiByIdAsync(long id) =>
            _db.SupplierKpis.FirstOrDefaultAsync(x => x.KpiID == id && !x.IsDeleted);

        public Task<List<SupplierKpi>> GetKpisBySupplierIdAsync(long supplierId) =>
            _db.SupplierKpis.Where(x => x.SupplierID == supplierId && !x.IsDeleted).ToListAsync();

        public async Task<SupplierKpi?> UpdateKpiAsync(SupplierKpi kpi)
        {
            _db.SupplierKpis.Update(kpi);
            await _db.SaveChangesAsync();
            return kpi;
        }

        public async Task<bool> DeleteKpiAsync(long id)
        {
            var kpi = await GetKpiByIdAsync(id);
            if (kpi == null) return false;

            kpi.IsDeleted = true; // Soft Delete
            _db.SupplierKpis.Update(kpi);
            await _db.SaveChangesAsync();
            return true;
        }

        // =======================
        // --- Scorecard Methods ---
        // =======================
        public async Task<Scorecard> AddScorecardAsync(Scorecard scorecard)
        {
            _db.Scorecards.Add(scorecard);
            await _db.SaveChangesAsync();
            return scorecard;
        }

        public Task<List<Scorecard>> GetAllScorecardsAsync() =>
            _db.Scorecards.Where(x => !x.IsDeleted).ToListAsync();

        public Task<Scorecard?> GetScorecardByIdAsync(long id) =>
            _db.Scorecards.FirstOrDefaultAsync(x => x.ScorecardID == id && !x.IsDeleted);

        public Task<List<Scorecard>> GetScorecardsBySupplierIdAsync(long supplierId) =>
            _db.Scorecards.Where(x => x.SupplierID == supplierId && !x.IsDeleted).ToListAsync();

        public async Task<Scorecard?> UpdateScorecardAsync(Scorecard scorecard)
        {
            _db.Scorecards.Update(scorecard);
            await _db.SaveChangesAsync();
            return scorecard;
        }

        public async Task<bool> DeleteScorecardAsync(long id)
        {
            var scorecard = await GetScorecardByIdAsync(id);
            if (scorecard == null) return false;

            scorecard.IsDeleted = true; // Soft Delete
            _db.Scorecards.Update(scorecard);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}