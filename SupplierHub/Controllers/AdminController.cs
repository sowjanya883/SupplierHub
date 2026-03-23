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
		/// <param name="service">The admin service instance</param>
		public AdminController(IAdminService service)
		{
			_service = service;
		}

		/// <summary>
		/// Retrieves all system configurations.
		/// </summary>
		/// <returns>List of system configurations</returns>
		[HttpGet("system-configs")]
		public async Task<IActionResult> GetAllSystemConfigs() =>
			Ok(await _service.GetAllSystemConfigsAsync());

		/// <summary>
		/// Retrieves a specific system configuration by ID.
		/// </summary>
		/// <param name="id">The configuration ID</param>
		/// <returns>System configuration details if found; 404 if not found</returns>
		[HttpGet("system-configs/{id:long}")]
		public async Task<IActionResult> GetSystemConfigById(long id)
		{
			var result = await _service.GetSystemConfigByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves a system configuration by its config key.
		/// </summary>
		/// <param name="configKey">The configuration key</param>
		/// <returns>System configuration details if found; 404 if not found</returns>
		[HttpGet("system-configs/key/{configKey}")]
		public async Task<IActionResult> GetSystemConfigByKey(string configKey)
		{
			var result = await _service.GetSystemConfigByKeyAsync(configKey);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Creates a new system configuration.
		/// </summary>
		/// <param name="dto">System configuration creation DTO</param>
		/// <returns>Created system configuration details</returns>
		[HttpPost("system-configs")]
		public async Task<IActionResult> CreateSystemConfig(SystemConfigCreateDto dto) =>
			Ok(await _service.CreateSystemConfigAsync(dto));

		/// <summary>
		/// Updates an existing system configuration.
		/// </summary>
		/// <param name="dto">System configuration update DTO</param>
		/// <returns>Updated system configuration details if found; 404 if not found</returns>
		[HttpPut("system-configs")]
		public async Task<IActionResult> UpdateSystemConfig(SystemConfigUpdateDto dto)
		{
			var result = await _service.UpdateSystemConfigAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves all approval rules.
		/// </summary>
		/// <returns>List of approval rules</returns>
		[HttpGet("approval-rules")]
		public async Task<IActionResult> GetAllApprovalRules() =>
			Ok(await _service.GetAllApprovalRulesAsync());

		/// <summary>
		/// Retrieves a specific approval rule by ID.
		/// </summary>
		/// <param name="id">The approval rule ID</param>
		/// <returns>Approval rule details if found; 404 if not found</returns>
		[HttpGet("approval-rules/{id:long}")]
		public async Task<IActionResult> GetApprovalRuleById(long id)
		{
			var result = await _service.GetApprovalRuleByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}

		/// <summary>
		/// Retrieves all approval rules for a specific scope.
		/// </summary>
		/// <param name="scope">The scope to filter by</param>
		/// <returns>List of approval rules for the given scope</returns>
		[HttpGet("approval-rules/scope/{scope}")]
		public async Task<IActionResult> GetApprovalRulesByScope(string scope) =>
			Ok(await _service.GetApprovalRulesByScopeAsync(scope));

		/// <summary>
		/// Creates a new approval rule.
		/// </summary>
		/// <param name="dto">Approval rule creation DTO</param>
		/// <returns>Created approval rule details</returns>
		[HttpPost("approval-rules")]
		public async Task<IActionResult> CreateApprovalRule(ApprovalRuleCreateDto dto) =>
			Ok(await _service.CreateApprovalRuleAsync(dto));

		/// <summary>
		/// Updates an existing approval rule.
		/// </summary>
		/// <param name="dto">Approval rule update DTO</param>
		/// <returns>Updated approval rule details if found; 404 if not found</returns>
		[HttpPut("approval-rules")]
		public async Task<IActionResult> UpdateApprovalRule(ApprovalRuleUpdateDto dto)
		{
			var result = await _service.UpdateApprovalRuleAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}
	}
}
