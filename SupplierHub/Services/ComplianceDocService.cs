using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class ComplianceDocService : IComplianceDocService
	{
		private readonly IComplianceDocRepository _repo;
		private readonly IMapper _mapper;

		public ComplianceDocService(IComplianceDocRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<ComplianceDocGetByIdDto> CreateAsync(
			ComplianceDocCreateDto dto,
			CancellationToken ct)
		{
			var entity = _mapper.Map<ComplianceDoc>(dto);
			entity.IsDeleted = false;
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);
			return _mapper.Map<ComplianceDocGetByIdDto>(created);
		}

		public async Task<ComplianceDocGetByIdDto?> GetByIdAsync(long docId, CancellationToken ct)
		{
			var doc = await _repo.GetByIdAsync(docId, ct);
			return _mapper.Map<ComplianceDocGetByIdDto>(doc);
		}

		public async Task<IEnumerable<ComplianceDocGetAllDto>> GetAllAsync(CancellationToken ct)
		{
			var docs = await _repo.GetAllAsync(ct);
			return _mapper.Map<List<ComplianceDocGetAllDto>>(docs);
		}

		public async Task<ComplianceDocGetByIdDto?> UpdateAsync(
			ComplianceDocUpdateDto dto,
			CancellationToken ct)
		{
			var doc = await _repo.GetByIdAsync(dto.DocID, ct);
			if (doc == null)
				return null;

			_mapper.Map(dto, doc);
			doc.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(doc, ct);
			return _mapper.Map<ComplianceDocGetByIdDto>(updated);
		}

		public async Task<bool> DeleteAsync(
			ComplianceDocDeleteDto dto,
			CancellationToken ct)
		{
			return await _repo.DeleteAsync(dto, ct);
		}
	}
}