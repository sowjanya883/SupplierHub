using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SupplierHub.DTOs.ApprovalRuleDTO;
using SupplierHub.DTOs.SystemConfigDTO;
using SupplierHub.DTOs.RoleDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	/// <summary>
	/// Controller for managing admin operations including system configurations, approval rules, and user roles.
	/// All endpoints require Admin authorization.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	//[Authorize(Roles = "Admin")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminService _service;

		/// <summary>
		/// Initializes a new instance of the AdminController class.
		/// </summary>

		public AdminController(IAdminService service)
		{
			_service = service;
		}

		/// <summary>
		/// Retrieves all system configurations.
		/// </summary>

		[HttpGet("system-configs")]
		public async Task<IActionResult> GetAllSystemConfigs()
		{
			try
			{
				var result = await _service.GetAllSystemConfigsAsync();
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving system configurations.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Retrieves a specific system configuration by ID.
		/// </summary>

		[HttpGet("system-configs/{id:long}")]
		public async Task<IActionResult> GetSystemConfigById(long id)
		{
			try
			{
				var result = await _service.GetSystemConfigByIdAsync(id);
				return result == null ? NotFound(new { message = $"System configuration with ID {id} not found." }) : Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving system configuration.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Retrieves a system configuration by its config key.
		/// </summary>

		[HttpGet("system-configs/key/{configKey}")]
		public async Task<IActionResult> GetSystemConfigByKey(string configKey)
		{
			try
			{
				var result = await _service.GetSystemConfigByKeyAsync(configKey);
				return result == null ? NotFound(new { message = $"System configuration with key '{configKey}' not found." }) : Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving system configuration.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Creates a new system configuration.
		/// </summary>

		[HttpPost("system-configs")]
		public async Task<IActionResult> CreateSystemConfig(SystemConfigCreateDto dto)
		{
			try
			{
				var result = await _service.CreateSystemConfigAsync(dto);
				return Ok(new { message = "System configuration created successfully.", data = result });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating system configuration.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Updates an existing system configuration.
		/// </summary>


		[HttpPut("system-configs")]
		public async Task<IActionResult> UpdateSystemConfig(SystemConfigUpdateDto dto)
		{
			try
			{
				var result = await _service.UpdateSystemConfigAsync(dto);
				return result == null ? NotFound(new { message = $"System configuration with ID {dto.ConfigID} not found." }) : Ok(new { message = "System configuration updated successfully.", data = result });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while updating system configuration.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Retrieves all approval rules.
		/// </summary>

		[HttpGet("approval-rules")]
		public async Task<IActionResult> GetAllApprovalRules()
		{
			try
			{
				var result = await _service.GetAllApprovalRulesAsync();
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving approval rules.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Retrieves a specific approval rule by ID.
		/// </summary>

		[HttpGet("approval-rules/{id:long}")]
		public async Task<IActionResult> GetApprovalRuleById(long id)
		{
			try
			{
				var result = await _service.GetApprovalRuleByIdAsync(id);
				return result == null ? NotFound(new { message = $"Approval rule with ID {id} not found." }) : Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving approval rule.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Retrieves all approval rules for a specific scope.
		/// </summary>

		[HttpGet("approval-rules/scope/{scope}")]
		public async Task<IActionResult> GetApprovalRulesByScope(string scope)
		{
			try
			{
				var result = await _service.GetApprovalRulesByScopeAsync(scope);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving approval rules.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Creates a new approval rule.
		/// </summary>

		[HttpPost("approval-rules")]
		public async Task<IActionResult> CreateApprovalRule(ApprovalRuleCreateDto dto)
		{
			try
			{
				var result = await _service.CreateApprovalRuleAsync(dto);
				return Ok(new { message = "Approval rule created successfully.", data = result });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while creating approval rule.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Updates an existing approval rule.
		/// </summary>

		[HttpPut("approval-rules")]
		public async Task<IActionResult> UpdateApprovalRule(ApprovalRuleUpdateDto dto)
		{
			var result = await _service.UpdateApprovalRuleAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Assigns a role to a user (Admin only).
		/// </summary>
		[HttpPost("assign-role")]
		public async Task<IActionResult> AssignRole(AssignRoleDto dto)
		{
			try
			{
				var result = await _service.AssignRoleAsync(dto);
				return Ok(new { message = "Role assigned successfully.", data = result });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while assigning role.",
					error = ex.Message
				});
			}
		}

		/// <summary>
		/// Removes a role from a user (Admin only).
		/// </summary>
		[HttpDelete("delete-role")]
		public async Task<IActionResult> DeleteRole(DeleteRoleDto dto)
		{
			try
			{
				var result = await _service.DeleteRoleAsync(dto);
				if (!result)
					return NotFound(new { message = $"No active role assignment found for User {dto.UserID} / Role {dto.RoleID}." });

				return Ok(new { message = "Role removed successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while removing role.",
					error = ex.Message
				});
			}
		}
	}
}
