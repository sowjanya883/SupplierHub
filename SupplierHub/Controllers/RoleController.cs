using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupplierHub.DTOs.RoleDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService service, ILogger<RoleController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _service.GetAllAsync(includeDeleted, ct);
            return Ok(items);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id, CancellationToken ct = default)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto, ct);
                return Ok(created);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return Problem(ex.Message);
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateRoleDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto, ct);
                if (updated == null)
                    return NotFound();
                return Ok(updated);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating role {RoleId}", id);
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            try
            {
                var ok = await _service.SoftDeleteAsync(id, ct);
                if (!ok) return NotFound();
                return Ok();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", id);
                return Problem(ex.Message);
            }
        }
    }
}
