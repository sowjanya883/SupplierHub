using AutoMapper;
using SupplierHub.DTOs.ApprovalRuleDTO;
using SupplierHub.DTOs.SystemConfigDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class AdminService : IAdminService
	{
		private readonly IAdminRepository _repo;
		private readonly IMapper _mapper;

		public AdminService(IAdminRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		/// <summary>
		/// Retrieves a system configuration by its ID.
		/// </summary>
		public async Task<SystemConfigReadDto?> GetSystemConfigByIdAsync(long id)
		{
			var entity = await _repo.GetSystemConfigByIdAsync(id);
			return entity == null ? null : _mapper.Map<SystemConfigReadDto>(entity);
		}

		/// <summary>
		/// Retrieves a system configuration by its config key.
		/// </summary>
		public async Task<SystemConfigReadDto?> GetSystemConfigByKeyAsync(string configKey)
		{
			var entity = await _repo.GetSystemConfigByKeyAsync(configKey);
			return entity == null ? null : _mapper.Map<SystemConfigReadDto>(entity);
		}

		/// <summary>
		/// Retrieves all system configurations.
		/// </summary>
		public async Task<List<SystemConfigReadDto>> GetAllSystemConfigsAsync()
		{
			var entities = await _repo.GetAllSystemConfigsAsync();
			return _mapper.Map<List<SystemConfigReadDto>>(entities);
		}

		/// <summary>
		/// Creates a new system configuration.
		/// </summary>
		public async Task<SystemConfigReadDto> CreateSystemConfigAsync(SystemConfigCreateDto dto)
		{
			var entity = _mapper.Map<SystemConfig>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddSystemConfigAsync(entity);
			return _mapper.Map<SystemConfigReadDto>(saved);
		}

		/// <summary>
		/// Updates an existing system configuration.
		/// </summary>
		public async Task<SystemConfigReadDto?> UpdateSystemConfigAsync(SystemConfigUpdateDto dto)
		{
			var existing = await _repo.GetSystemConfigByIdAsync(dto.ConfigID);
			if (existing == null) return null;

			_mapper.Map(dto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateSystemConfigAsync(existing);
			return updated == null ? null : _mapper.Map<SystemConfigReadDto>(updated);
		}

		/// <summary>
		/// Retrieves an approval rule by its ID.
		/// </summary>
		public async Task<ApprovalRuleReadDto?> GetApprovalRuleByIdAsync(long id)
		{
			var entity = await _repo.GetApprovalRuleByIdAsync(id);
			return entity == null ? null : _mapper.Map<ApprovalRuleReadDto>(entity);
		}

		/// <summary>
		/// Retrieves all approval rules.
		/// </summary>
		public async Task<List<ApprovalRuleReadDto>> GetAllApprovalRulesAsync()
		{
			var entities = await _repo.GetAllApprovalRulesAsync();
			return _mapper.Map<List<ApprovalRuleReadDto>>(entities);
		}

		/// <summary>
		/// Retrieves all approval rules for a specific scope.
		/// </summary>
		public async Task<List<ApprovalRuleReadDto>> GetApprovalRulesByScopeAsync(string scope)
		{
			var entities = await _repo.GetApprovalRulesByScopeAsync(scope);
			return _mapper.Map<List<ApprovalRuleReadDto>>(entities);
		}

		/// <summary>
		/// Creates a new approval rule.
		/// </summary>
		public async Task<ApprovalRuleReadDto> CreateApprovalRuleAsync(ApprovalRuleCreateDto dto)
		{
			var entity = _mapper.Map<ApprovalRule>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddApprovalRuleAsync(entity);
			return _mapper.Map<ApprovalRuleReadDto>(saved);
		}

		/// <summary>
		/// Updates an existing approval rule.
		/// </summary>
		public async Task<ApprovalRuleReadDto?> UpdateApprovalRuleAsync(ApprovalRuleUpdateDto dto)
		{
			var existing = await _repo.GetApprovalRuleByIdAsync(dto.RuleID);
			if (existing == null) return null;

			_mapper.Map(dto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateApprovalRuleAsync(existing);
			return updated == null ? null : _mapper.Map<ApprovalRuleReadDto>(updated);
		}
	}
}
