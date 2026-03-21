using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class SupplierRiskService : ISupplierRiskService
	{
		private readonly ISupplierRiskRepository _repo;
		private readonly IMapper _mapper;

		public SupplierRiskService(ISupplierRiskRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<SupplierRiskGetByIdDto> CreateAsync(SupplierRiskCreateDto dto, CancellationToken ct)
		{
			var entity = _mapper.Map<SupplierRisk>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);
			return _mapper.Map<SupplierRiskGetByIdDto>(created);
		}

		public async Task<SupplierRiskGetByIdDto?> GetByIdAsync(long riskId, CancellationToken ct)
		{
			var risk = await _repo.GetByIdAsync(riskId, ct);
			return _mapper.Map<SupplierRiskGetByIdDto>(risk);
		}

		public async Task<IEnumerable<SupplierRiskGetAllDto>> GetAllAsync(CancellationToken ct)
		{
			var risks = await _repo.GetAllAsync(ct);
			return _mapper.Map<List<SupplierRiskGetAllDto>>(risks);
		}

		public async Task<SupplierRiskGetByIdDto?> UpdateAsync(SupplierRiskUpdateDto dto, CancellationToken ct)
		{
			var risk = await _repo.GetByIdAsync(dto.RiskID, ct);
			if (risk == null)
				return null;

			_mapper.Map(dto, risk);
			risk.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(risk, ct);
			return _mapper.Map<SupplierRiskGetByIdDto>(updated);
		}

		public async Task<bool> DeleteAsync(SupplierRiskDeleteDto dto, CancellationToken ct)
		{
			return await _repo.DeleteAsync(dto, ct);
		}
	}
}