using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SupplierHub.DTOs.GrnRefDTO;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.InspectionDTO;
using SupplierHub.DTOs.NcrDTO;
using SupplierHub.DTOs.GrnItemRefDTO;

namespace SupplierHub.Controllers
{
    [Authorize(Roles = "Supplier,Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReceivingQualityController : ControllerBase
    {
        private readonly IReceivingQualityService _service;

        public ReceivingQualityController(IReceivingQualityService service)
        {
            _service = service;
        }

        // =======================
        // --- GRN HEADER ---
        // =======================
        [HttpPost("grn")] // 1. CREATE
        public async Task<IActionResult> CreateGrn([FromBody] GrnCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _service.CreateGrnAsync(dto);
                return CreatedAtAction(nameof(GetGrn), new { id = created.GrnID }, created);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpGet("grn")] // 2. GET ALL
        public async Task<IActionResult> GetAllGrns() =>
            Ok(await _service.GetAllGrnsAsync());

        [HttpGet("grn/{id:long}")] // 3. GET BY ID
        public async Task<IActionResult> GetGrn(long id)
        {
            var result = await _service.GetGrnByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("grn/{id:long}")] // 4. UPDATE
        public async Task<IActionResult> UpdateGrn(long id, [FromBody] GrnUpdateDto dto)
        {
            var result = await _service.UpdateGrnAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("grn/{id:long}")] // 5. DELETE
        public async Task<IActionResult> DeleteGrn(long id)
        {
            var success = await _service.DeleteGrnAsync(id);
            return success ? NoContent() : NotFound();
        }

        // =======================
        // --- GRN ITEMS ---
        // =======================
        [HttpPost("grn-item")] // 1. CREATE
        public async Task<IActionResult> CreateGrnItem([FromBody] GrnItemCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.AddGrnItemAsync(dto);
            return Ok(created);
        }

        [HttpGet("grn-item")] // 2. GET ALL
        public async Task<IActionResult> GetAllGrnItems() =>
            Ok(await _service.GetAllGrnItemsAsync());

        [HttpGet("grn-item/{id:long}")] // 3. GET BY ID
        public async Task<IActionResult> GetGrnItemById(long id)
        {
            var result = await _service.GetGrnItemByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("grn-item/grn/{grnId:long}")] // BONUS: Get by GRN ID
        public async Task<IActionResult> GetGrnItemsByGrn(long grnId) =>
            Ok(await _service.GetGrnItemsByGrnIdAsync(grnId));

        [HttpPut("grn-item/{id:long}")] // 4. UPDATE
        public async Task<IActionResult> UpdateGrnItem(long id, [FromBody] GrnItemUpdateDto dto)
        {
            var result = await _service.UpdateGrnItemAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("grn-item/{id:long}")] // 5. DELETE
        public async Task<IActionResult> DeleteGrnItem(long id)
        {
            var success = await _service.DeleteGrnItemAsync(id);
            return success ? NoContent() : NotFound();
        }

        // =======================
        // --- INSPECTIONS ---
        // =======================
        [HttpPost("inspections")] // 1. CREATE
        public async Task<IActionResult> CreateInspection([FromBody] InspectionCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _service.CreateInspectionAsync(dto);
                return Ok(created);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpGet("inspections")] // 2. GET ALL
        public async Task<IActionResult> GetAllInspections() =>
            Ok(await _service.GetAllInspectionsAsync());

        [HttpGet("inspections/{id:long}")] // 3. GET BY ID
        public async Task<IActionResult> GetInspectionById(long id)
        {
            var result = await _service.GetInspectionByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("inspections/item/{grnItemId:long}")] // BONUS: Get by Item ID
        public async Task<IActionResult> GetInspectionsByItem(long grnItemId) =>
            Ok(await _service.GetInspectionsByItemIdAsync(grnItemId));

        [HttpPut("inspections/{id:long}")] // 4. UPDATE
        public async Task<IActionResult> UpdateInspection(long id, [FromBody] InspectionUpdateDto dto)
        {
            var result = await _service.UpdateInspectionAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("inspections/{id:long}")] // 5. DELETE
        public async Task<IActionResult> DeleteInspection(long id)
        {
            var success = await _service.DeleteInspectionAsync(id);
            return success ? NoContent() : NotFound();
        }

        // =======================
        // --- NCR ---
        // =======================
        [HttpPost("ncr")] // 1. CREATE
        public async Task<IActionResult> CreateNcr([FromBody] NcrCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _service.CreateNcrAsync(dto);
                return Ok(created);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpGet("ncr")] // 2. GET ALL
        public async Task<IActionResult> GetAllNcrs() =>
            Ok(await _service.GetAllNcrsAsync());

        [HttpGet("ncr/{id:long}")] // 3. GET BY ID
        public async Task<IActionResult> GetNcrById(long id)
        {
            var result = await _service.GetNcrByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("ncr/item/{grnItemId:long}")] // BONUS: Get by Item ID
        public async Task<IActionResult> GetNcrsByItem(long grnItemId) =>
            Ok(await _service.GetNcrsByItemIdAsync(grnItemId));

        [HttpPut("ncr/{id:long}")] // 4. UPDATE
        public async Task<IActionResult> UpdateNcr(long id, [FromBody] NcrUpdateDto dto)
        {
            var result = await _service.UpdateNcrAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("ncr/{id:long}")] // 5. DELETE
        public async Task<IActionResult> DeleteNcr(long id)
        {
            var success = await _service.DeleteNcrAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}