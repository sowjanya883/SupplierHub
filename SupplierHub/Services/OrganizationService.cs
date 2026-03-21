using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.OrganizationDTO;
using SupplierHub.Models;

namespace SupplierHub.Services
{
	public class OrganizationService : IOrganizationService
	{
		private readonly IOrganizationRepository _repo;
		private readonly IMapper _mapper;

		public OrganizationService(IOrganizationRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<OrganizationGetByIdDto> CreateAsync(
			OrganizationCreateDto dto,
			CancellationToken ct)
		{
			var entity = _mapper.Map<Organization>(dto);

			entity.IsDeleted = false;
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);
			return _mapper.Map<OrganizationGetByIdDto>(created);
		}

		public async Task<OrganizationGetByIdDto?> GetByIdAsync(long orgId, CancellationToken ct)
		{
			var org = await _repo.GetByIdAsync(orgId, ct);
			return _mapper.Map<OrganizationGetByIdDto>(org);
		}

		public async Task<IEnumerable<OrganizationGetAllDto>> GetAllAsync(CancellationToken ct)
		{
			var orgs = await _repo.GetAllAsync(ct);
			return _mapper.Map<List<OrganizationGetAllDto>>(orgs);
		}

		public async Task<OrganizationGetByIdDto?> UpdateAsync(
			OrganizationUpdateDto dto,
			CancellationToken ct)
		{
			var org = await _repo.GetByIdAsync(dto.OrgID, ct);
			if (org == null)
				return null;

			_mapper.Map(dto, org);
			org.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(org, ct);
			return _mapper.Map<OrganizationGetByIdDto>(updated);
		}

		public async Task<bool> DeleteAsync(
			OrganizationDeleteDto dto,
			CancellationToken ct)
		{
			return await _repo.DeleteAsync(dto, ct);
		}
	}
}