using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.GrnItemRefDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
    [Authorize(Roles = "Admin,SupplierUser,ReceivingUser,WarehouseManager")]
    [ApiController]
    [Route("api/[controller]")]
    public class GrnItemController : ControllerBase
    {
        private readonly IGrnItemService _service;

        public GrnItemController(IGrnItemService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GrnItemCreateDto dto)
        {
            try
            {
                var created = await _service.AddGrnItemAsync(dto);
                return Ok(new { message = "GRN Item created successfully.", data = created });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _service.GetAllGrnItemsAsync();
                return Ok(new { message = "GRN Items retrieved successfully.", data = items });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _service.GetGrnItemByIdAsync(id);
                if (result == null) return NotFound(new { message = $"GRN Item with ID {id} not found." });
                return Ok(new { message = "GRN Item retrieved successfully.", data = result });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpGet("grn/{grnId:long}")]
        public async Task<IActionResult> GetByGrnId(long grnId)
        {
            try
            {
                var results = await _service.GetGrnItemsByGrnIdAsync(grnId);
                return Ok(new { message = $"Items for GRN {grnId} retrieved.", data = results });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] GrnItemUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateGrnItemAsync(id, dto);
                if (updated == null) return NotFound(new { message = $"GRN Item with ID {id} not found." });
                return Ok(new { message = "GRN Item updated successfully.", data = updated });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var success = await _service.DeleteGrnItemAsync(id);
                if (!success) return NotFound(new { message = $"No matching GRN Item found with ID {id}." });
                return Ok(new { message = "GRN Item deleted successfully." });
            }
            catch (Exception ex) { return StatusCode(500, new { message = "An error occurred.", error = ex.Message }); }
        }
    }
}