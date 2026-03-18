using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupplierHub.DTOs.RolePermissionDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _service;
        private readonly ILogger<RolePermissionController> _logger;

        public RolePermissionController(IRolePermissionService service, ILogger<RolePermissionController> logger)
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

        [HttpGet("role/{roleId:long}")]
        public async Task<IActionResult> GetByRole(long roleId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _service.GetByRoleAsync(roleId, includeDeleted, ct);
            return Ok(items);
        }

        [HttpGet("{roleId:long}/{permissionId:long}")]
        public async Task<IActionResult> GetByIds(long roleId, long permissionId, CancellationToken ct = default)
        {
            var item = await _service.GetByIdsAsync(roleId, permissionId, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRolePermissionDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto, ct);
                return Ok(created);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating role-permission");
                return Problem(ex.Message);
            }
        }

        [HttpPut("{roleId:long}/{permissionId:long}")]
        public async Task<IActionResult> Update(long roleId, long permissionId, [FromBody] UpdateRolePermissionDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(roleId, permissionId, dto, ct);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating role-permission {RoleId}-{PermissionId}", roleId, permissionId);
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{roleId:long}/{permissionId:long}")]
        public async Task<IActionResult> Delete(long roleId, long permissionId, CancellationToken ct = default)
        {
            try
            {
                var ok = await _service.SoftDeleteAsync(roleId, permissionId, ct);
                if (!ok) return NotFound();
                return Ok();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting role-permission {RoleId}-{PermissionId}", roleId, permissionId);
                return Problem(ex.Message);
            }
        }
    }
}
