using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.SupplierKpiDTO;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.ScorecardDTO;

namespace SupplierHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceService _service;

        public PerformanceController(IPerformanceService service)
        {
            _service = service;
        }

        // =======================
        // --- SUPPLIER KPIs ---
        // =======================

        [HttpPost("kpi")] // 1. CREATE
        public async Task<IActionResult> CreateKpi(SupplierKpiCreateDto dto) =>
            Ok(await _service.CreateKpiAsync(dto));

        [HttpGet("kpi")] // 2. GET ALL
        public async Task<IActionResult> GetAllKpis() =>
            Ok(await _service.GetAllKpisAsync());

        [HttpGet("kpi/{id:long}")] // 3. GET BY ID
        public async Task<IActionResult> GetKpiById(long id)
        {
            var result = await _service.GetKpiByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("kpi/supplier/{supplierId:long}")] // BONUS: Get by Supplier ID
        public async Task<IActionResult> GetKpisBySupplier(long supplierId) =>
            Ok(await _service.GetKpisBySupplierAsync(supplierId));

        [HttpPut("kpi/{id:long}")] // 4. UPDATE
        public async Task<IActionResult> UpdateKpi(long id, [FromBody] SupplierKpiUpdateDto dto)
        {
            var result = await _service.UpdateKpiAsync(id, dto);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpDelete("kpi/{id:long}")] // 5. DELETE
        public async Task<IActionResult> DeleteKpi(long id)
        {
            var success = await _service.DeleteKpiAsync(id);
            return success ? NoContent() : NotFound();
        }

        // =======================
        // --- SCORECARDS ---
        // =======================

        [HttpPost("scorecard")] // 1. CREATE
        public async Task<IActionResult> CreateScorecard(ScorecardCreateDto dto) =>
            Ok(await _service.CreateScorecardAsync(dto));

        [HttpGet("scorecard")] // 2. GET ALL
        public async Task<IActionResult> GetAllScorecards() =>
            Ok(await _service.GetAllScorecardsAsync());

        [HttpGet("scorecard/{id:long}")] // 3. GET BY ID
        public async Task<IActionResult> GetScorecardById(long id)
        {
            var result = await _service.GetScorecardByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("scorecard/supplier/{supplierId:long}")] // BONUS: Get by Supplier ID
        public async Task<IActionResult> GetScorecardsBySupplier(long supplierId) =>
            Ok(await _service.GetScorecardsBySupplierAsync(supplierId));

        [HttpPut("scorecard/{id:long}")] // 4. UPDATE
        public async Task<IActionResult> UpdateScorecard(long id, [FromBody] ScorecardUpdateDto dto)
        {
            var result = await _service.UpdateScorecardAsync(id, dto);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpDelete("scorecard/{id:long}")] // 5. DELETE
        public async Task<IActionResult> DeleteScorecard(long id)
        {
            var success = await _service.DeleteScorecardAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}