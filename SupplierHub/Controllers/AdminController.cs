using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.ApprovalRuleDTO;
using SupplierHub.DTOs.SystemConfigDTO;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	/// <summary>
	/// Controller for managing admin operations including system configurations and approval rules.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminService _service;

		/// <summary>
		/// Initializes a new instance of the AdminController class.
		/// </summary>
<<<<<<< HEAD
=======
		/// <param name="service">The admin service instance</param>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		public AdminController(IAdminService service)
		{
			_service = service;
		}

		/// <summary>
		/// Retrieves all system configurations.
		/// </summary>
<<<<<<< HEAD
=======
		/// <returns>List of system configurations</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpGet("system-configs")]
		public async Task<IActionResult> GetAllSystemConfigs() =>
			Ok(await _service.GetAllSystemConfigsAsync());

		/// <summary>
		/// Retrieves a specific system configuration by ID.
		/// </summary>
<<<<<<< HEAD
=======
		/// <param name="id">The configuration ID</param>
		/// <returns>System configuration details if found; 404 if not found</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpGet("system-configs/{id:long}")]
		public async Task<IActionResult> GetSystemConfigById(long id)
		{
			var result = await _service.GetSystemConfigByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves a system configuration by its config key.
		/// </summary>
<<<<<<< HEAD
=======
		/// <param name="configKey">The configuration key</param>
		/// <returns>System configuration details if found; 404 if not found</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpGet("system-configs/key/{configKey}")]
		public async Task<IActionResult> GetSystemConfigByKey(string configKey)
		{
			var result = await _service.GetSystemConfigByKeyAsync(configKey);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Creates a new system configuration.
		/// </summary>
<<<<<<< HEAD
=======
		/// <param name="dto">System configuration creation DTO</param>
		/// <returns>Created system configuration details</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpPost("system-configs")]
		public async Task<IActionResult> CreateSystemConfig(SystemConfigCreateDto dto) =>
			Ok(await _service.CreateSystemConfigAsync(dto));

		/// <summary>
		/// Updates an existing system configuration.
		/// </summary>
<<<<<<< HEAD
=======
		/// <param name="dto">System configuration update DTO</param>
		/// <returns>Updated system configuration details if found; 404 if not found</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpPut("system-configs")]
		public async Task<IActionResult> UpdateSystemConfig(SystemConfigUpdateDto dto)
		{
			var result = await _service.UpdateSystemConfigAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves all approval rules.
		/// </summary>
<<<<<<< HEAD
=======
		/// <returns>List of approval rules</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpGet("approval-rules")]
		public async Task<IActionResult> GetAllApprovalRules() =>
			Ok(await _service.GetAllApprovalRulesAsync());

		/// <summary>
		/// Retrieves a specific approval rule by ID.
		/// </summary>
<<<<<<< HEAD
=======
		/// <param name="id">The approval rule ID</param>
		/// <returns>Approval rule details if found; 404 if not found</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpGet("approval-rules/{id:long}")]
		public async Task<IActionResult> GetApprovalRuleById(long id)
		{
			var result = await _service.GetApprovalRuleByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves all approval rules for a specific scope.
		/// </summary>
<<<<<<< HEAD
=======
		/// <param name="scope">The scope to filter by</param>
		/// <returns>List of approval rules for the given scope</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpGet("approval-rules/scope/{scope}")]
		public async Task<IActionResult> GetApprovalRulesByScope(string scope) =>
			Ok(await _service.GetApprovalRulesByScopeAsync(scope));

		/// <summary>
		/// Creates a new approval rule.
		/// </summary>
<<<<<<< HEAD
=======
		/// <param name="dto">Approval rule creation DTO</param>
		/// <returns>Created approval rule details</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpPost("approval-rules")]
		public async Task<IActionResult> CreateApprovalRule(ApprovalRuleCreateDto dto) =>
			Ok(await _service.CreateApprovalRuleAsync(dto));

		/// <summary>
		/// Updates an existing approval rule.
		/// </summary>
<<<<<<< HEAD
=======
		/// <param name="dto">Approval rule update DTO</param>
		/// <returns>Updated approval rule details if found; 404 if not found</returns>
>>>>>>> 2c384cec079b38035b6f3d79275d861ba17d7854
		[HttpPut("approval-rules")]
		public async Task<IActionResult> UpdateApprovalRule(ApprovalRuleUpdateDto dto)
		{
			var result = await _service.UpdateApprovalRuleAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}
	}
}
