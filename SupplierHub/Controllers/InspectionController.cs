using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.InspectionDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
    [Authorize(Roles = "Admin,SupplierUser,ReceivingUser,WarehouseManager,ComplianceOfficer")]
    [ApiController]
    [Route("api/[controller]")]
    public class InspectionController : ControllerBase
    {
        private readonly IInspectionService _service;

        public InspectionController(IInspectionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InspectionCreateDto dto)
        {
            try
            {
                var created = await _service.CreateInspectionAsync(dto);
                return Ok(new { message = "Inspection created successfully.", data = created });
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var inspections = await _service.GetAllInspectionsAsync();
                return Ok(new { message = "Inspections retrieved successfully.", data = inspections });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _service.GetInspectionByIdAsync(id);
                if (result == null) return NotFound(new { message = $"Inspection with ID {id} not found." });
                return Ok(new { message = "Inspection retrieved successfully.", data = result });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet("item/{grnItemId:long}")]
        public async Task<IActionResult> GetByItemId(long grnItemId)
        {
            try
            {
                var results = await _service.GetInspectionsByItemIdAsync(grnItemId);
                return Ok(new { message = $"Inspections for GRN Item {grnItemId} retrieved.", data = results });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] InspectionUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateInspectionAsync(id, dto);
                if (updated == null) return NotFound(new { message = $"Inspection with ID {id} not found." });
                return Ok(new { message = "Inspection updated successfully.", data = updated });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var success = await _service.DeleteInspectionAsync(id);
                if (!success) return NotFound(new { message = $"No matching Inspection found with ID {id}." });
                return Ok(new { message = "Inspection deleted successfully." });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }
    }
}