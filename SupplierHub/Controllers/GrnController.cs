using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierHub.DTOs.GrnRefDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
    [Authorize(Roles = "Admin,SupplierUser,ReceivingUser,WarehouseManager,AccountsPayable,Buyer")]
    [ApiController]
    [Route("api/[controller]")]
    public class GrnController : ControllerBase
    {
        private readonly IGrnService _service;

        public GrnController(IGrnService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GrnCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid GRN payload — check required fields.", errors = ModelState });
            try
            {
                var created = await _service.CreateGrnAsync(dto);
                return Ok(new { message = "GRN created successfully.", data = created });
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (DbUpdateException ex) { return BadRequest(new { message = "Could not create GRN — check that PO and ASN IDs exist.", detail = ex.InnerException?.Message ?? ex.Message }); }
            catch (Exception ex) { return BadRequest(new { message = "Could not create GRN.", detail = ex.Message }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var grns = await _service.GetAllGrnsAsync();
                return Ok(new { message = "GRNs retrieved successfully.", data = grns });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _service.GetGrnByIdAsync(id);
                if (result == null) return NotFound(new { message = $"GRN with ID {id} not found." });
                return Ok(new { message = "GRN retrieved successfully.", data = result });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] GrnUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateGrnAsync(id, dto);
                if (updated == null) return NotFound(new { message = $"GRN with ID {id} not found." });
                return Ok(new { message = "GRN updated successfully.", data = updated });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var success = await _service.DeleteGrnAsync(id);
                if (!success) return NotFound(new { message = $"No matching GRN found with ID {id}." });
                return Ok(new { message = "GRN deleted successfully." });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }
    }
}