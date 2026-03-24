using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class AdminRepository : IAdminRepository
	{
		private readonly AppDbContext _db;

		public AdminRepository(AppDbContext db)
		{
			_db = db;
		}

		/// <summary>
		/// Retrieves a system configuration by its ID.
		/// </summary>
		public Task<SystemConfig?> GetSystemConfigByIdAsync(long id) =>
			_db.SystemConfigs.FirstOrDefaultAsync(x => x.ConfigID == id);

		/// <summary>
		/// Retrieves a system configuration by its config key.
		/// </summary>
		public Task<SystemConfig?> GetSystemConfigByKeyAsync(string configKey) =>
			_db.SystemConfigs.FirstOrDefaultAsync(x => x.ConfigKey == configKey);

		/// <summary>
		/// Retrieves all system configurations.
		/// </summary>
		public Task<List<SystemConfig>> GetAllSystemConfigsAsync() =>
			_db.SystemConfigs.ToListAsync();

		/// <summary>
		/// Adds a new system configuration to the database.
		/// </summary>
		public async Task<SystemConfig> AddSystemConfigAsync(SystemConfig config)
		{
			_db.SystemConfigs.Add(config);
			await _db.SaveChangesAsync();
			return config;
		}

		/// <summary>
		/// Updates an existing system configuration in the database.
		/// </summary>
		public async Task<SystemConfig?> UpdateSystemConfigAsync(SystemConfig config)
		{
			_db.SystemConfigs.Update(config);
			await _db.SaveChangesAsync();
			return config;
		}

		/// <summary>
		/// Retrieves an approval rule by its ID.
		/// </summary>
		public Task<ApprovalRule?> GetApprovalRuleByIdAsync(long id) =>
			_db.ApprovalRules.FirstOrDefaultAsync(x => x.RuleID == id);

		/// <summary>
		/// Retrieves all approval rules.
		/// </summary>
		public Task<List<ApprovalRule>> GetAllApprovalRulesAsync() =>
			_db.ApprovalRules.ToListAsync();

		/// <summary>
		/// Retrieves all approval rules for a specific scope.
		/// </summary>
		public Task<List<ApprovalRule>> GetApprovalRulesByScopeAsync(string scope) =>
			_db.ApprovalRules.Where(x => x.Scope == scope).ToListAsync();

		/// <summary>
		/// Adds a new approval rule to the database.
		/// </summary>
		public async Task<ApprovalRule> AddApprovalRuleAsync(ApprovalRule rule)
		{
			_db.ApprovalRules.Add(rule);
			await _db.SaveChangesAsync();
			return rule;
		}

		/// <summary>
		/// Updates an existing approval rule in the database.
		/// </summary>
		public async Task<ApprovalRule?> UpdateApprovalRuleAsync(ApprovalRule rule)
		{
			_db.ApprovalRules.Update(rule);
			await _db.SaveChangesAsync();
			return rule;
		}
	}
}
