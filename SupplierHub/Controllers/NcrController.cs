using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.NcrDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
    [Authorize(Roles = "Admin,ReceivingUser,ComplianceOfficer,WarehouseManager,SupplierUser")]
    [ApiController]
    [Route("api/[controller]")]
    public class NcrController : ControllerBase
    {
        private readonly INcrService _service;

        public NcrController(INcrService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NcrCreateDto dto)
        {
            try
            {
                var created = await _service.CreateNcrAsync(dto);
                return Ok(new { message = "NCR created successfully.", data = created });
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ncrs = await _service.GetAllNcrsAsync();
                return Ok(new { message = "NCRs retrieved successfully.", data = ncrs });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _service.GetNcrByIdAsync(id);
                if (result == null) return NotFound(new { message = $"NCR with ID {id} not found." });
                return Ok(new { message = "NCR retrieved successfully.", data = result });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet("item/{grnItemId:long}")]
        public async Task<IActionResult> GetByItemId(long grnItemId)
        {
            try
            {
                var results = await _service.GetNcrsByItemIdAsync(grnItemId);
                return Ok(new { message = $"NCRs for GRN Item {grnItemId} retrieved.", data = results });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] NcrUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateNcrAsync(id, dto);
                if (updated == null) return NotFound(new { message = $"NCR with ID {id} not found." });
                return Ok(new { message = "NCR updated successfully.", data = updated });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var success = await _service.DeleteNcrAsync(id);
                if (!success) return NotFound(new { message = $"No matching NCR found with ID {id}." });
                return Ok(new { message = "NCR deleted successfully." });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }
    }
}