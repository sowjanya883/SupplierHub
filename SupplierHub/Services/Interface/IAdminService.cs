using SupplierHub.DTOs.ApprovalRuleDTO;
using SupplierHub.DTOs.SystemConfigDTO;

namespace SupplierHub.Services.Interface
{
	public interface IAdminService
	{
		// SystemConfig
		Task<SystemConfigReadDto?> GetSystemConfigByIdAsync(long id);
		Task<SystemConfigReadDto?> GetSystemConfigByKeyAsync(string configKey);
		Task<List<SystemConfigReadDto>> GetAllSystemConfigsAsync();
		Task<SystemConfigReadDto> CreateSystemConfigAsync(SystemConfigCreateDto dto);
		Task<SystemConfigReadDto?> UpdateSystemConfigAsync(SystemConfigUpdateDto dto);

		// ApprovalRule
		Task<ApprovalRuleReadDto?> GetApprovalRuleByIdAsync(long id);
		Task<List<ApprovalRuleReadDto>> GetAllApprovalRulesAsync();
		Task<List<ApprovalRuleReadDto>> GetApprovalRulesByScopeAsync(string scope);
		Task<ApprovalRuleReadDto> CreateApprovalRuleAsync(ApprovalRuleCreateDto dto);
		Task<ApprovalRuleReadDto?> UpdateApprovalRuleAsync(ApprovalRuleUpdateDto dto);
	}
}
