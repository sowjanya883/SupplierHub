using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.ContractDTO;
using SupplierHub.Models;

namespace SupplierHub.Services
{
	public class ContractService : IContractService
	{
		private readonly IContractRepository _repo;
		private readonly IMapper _mapper;

		public ContractService(IContractRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<ContractGetByIdDto> CreateAsync(
			ContractCreateDto dto,
			CancellationToken ct)
		{
			var entity = _mapper.Map<Contract>(dto);

			entity.IsDeleted = false;
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);
			return _mapper.Map<ContractGetByIdDto>(created);
		}

		public async Task<ContractGetByIdDto?> GetByIdAsync(long contractId, CancellationToken ct)
		{
			var contract = await _repo.GetByIdAsync(contractId, ct);
			return _mapper.Map<ContractGetByIdDto>(contract);
		}

		public async Task<IEnumerable<ContractGetAllDto>> GetAllAsync(CancellationToken ct)
		{
			var contracts = await _repo.GetAllAsync(ct);
			return _mapper.Map<List<ContractGetAllDto>>(contracts);
		}

		public async Task<ContractGetByIdDto?> UpdateAsync(
			ContractUpdateDto dto,
			CancellationToken ct)
		{
			var contract = await _repo.GetByIdAsync(dto.ContractID, ct);
			if (contract == null)
				return null;

			_mapper.Map(dto, contract);
			contract.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(contract, ct);
			return _mapper.Map<ContractGetByIdDto>(updated);
		}

		public async Task<bool> DeleteAsync(
			ContractDeleteDto dto,
			CancellationToken ct)
		{
			return await _repo.DeleteAsync(dto, ct);
		}
	}
}