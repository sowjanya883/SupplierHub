using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupplierHub.DTOs.UserRoleDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _service;
        private readonly ILogger<UserRoleController> _logger;

        public UserRoleController(IUserRoleService service, ILogger<UserRoleController> logger)
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

        [HttpGet("user/{userId:long}")]
        public async Task<IActionResult> GetByUser(long userId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _service.GetByUserAsync(userId, includeDeleted, ct);
            return Ok(items);
        }

        [HttpGet("{userId:long}/{roleId:long}")]
        public async Task<IActionResult> GetByIds(long userId, long roleId, CancellationToken ct = default)
        {
            var item = await _service.GetByIdsAsync(userId, roleId, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRoleDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto, ct);
                return Ok(created);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating user-role");
                return Problem(ex.Message);
            }
        }

        [HttpPut("{userId:long}/{roleId:long}")]
        public async Task<IActionResult> Update(long userId, long roleId, [FromBody] UpdateUserRoleDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(userId, roleId, dto, ct);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating user-role {UserId}-{RoleId}", userId, roleId);
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{userId:long}/{roleId:long}")]
        public async Task<IActionResult> Delete(long userId, long roleId, CancellationToken ct = default)
        {
            try
            {
                var ok = await _service.SoftDeleteAsync(userId, roleId, ct);
                if (!ok) return NotFound();
                return Ok();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting user-role {UserId}-{RoleId}", userId, roleId);
                return Problem(ex.Message);
            }
        }
    }
}
