using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IAdminRepository
	{
		// SystemConfig
		Task<SystemConfig?> GetSystemConfigByIdAsync(long id);
		Task<SystemConfig?> GetSystemConfigByKeyAsync(string configKey);
		Task<List<SystemConfig>> GetAllSystemConfigsAsync();
		Task<SystemConfig> AddSystemConfigAsync(SystemConfig config);
		Task<SystemConfig?> UpdateSystemConfigAsync(SystemConfig config);

		// ApprovalRule
		Task<ApprovalRule?> GetApprovalRuleByIdAsync(long id);
		Task<List<ApprovalRule>> GetAllApprovalRulesAsync();
		Task<List<ApprovalRule>> GetApprovalRulesByScopeAsync(string scope);
		Task<ApprovalRule> AddApprovalRuleAsync(ApprovalRule rule);
		Task<ApprovalRule?> UpdateApprovalRuleAsync(ApprovalRule rule);
	}
}
