using AutoMapper;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.DTOs.OrganizationDTO;
using SupplierHub.DTOs.SupplierContactDTO;
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class SuppliersService : ISuppliersService
	{
		private readonly ISuppliersRepository _repo;
		private readonly IMapper _mapper;

		public SuppliersService(ISuppliersRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		// Get supplier by id
		public async Task<SupplierResponseDto?> GetSupplierByIdAsync(long id)
		{
			var entity = await _repo.GetSupplierByIdAsync(id);
			return entity == null ? null : _mapper.Map<SupplierResponseDto>(entity);
		}

		// Get all suppliers
		public async Task<List<SupplierListDto>> GetAllSuppliersAsync()
		{
			var entities = await _repo.GetAllSuppliersAsync();
			return _mapper.Map<List<SupplierListDto>>(entities);
		}

		// Create supplier
		public async Task<SupplierResponseDto> CreateSupplierAsync(SupplierCreateDto dto)
		{
			var entity = _mapper.Map<Supplier>(dto);
			entity.Status = "ACTIVE";
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddSupplierAsync(entity);
			return _mapper.Map<SupplierResponseDto>(saved);
		}

		// Update supplier
		public async Task<SupplierResponseDto?> UpdateSupplierAsync(SupplierUpdateDto dto)
		{
			var existing = await _repo.GetSupplierByIdAsync(dto.SupplierID);
			if (existing == null) return null;

			_mapper.Map(dto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateSupplierAsync(existing);
			return updated == null ? null : _mapper.Map<SupplierResponseDto>(updated);
		}

		// Get contacts by supplier
		public Task<List<SupplierContact>> GetContactsAsync(long supplierId) =>
			_repo.GetContactsBySupplierAsync(supplierId);

		// Add contact to supplier
		public async Task<SupplierContact> AddContactAsync(SupplierContact contact)
		{
			contact.CreatedOn = DateTime.UtcNow;
			contact.UpdatedOn = DateTime.UtcNow;
			contact.Status = "ACTIVE";
			contact.IsDeleted = false;
			return await _repo.AddContactAsync(contact);
		}

		// Update supplier contact
		public async Task<SupplierContactResponseDto?> UpdateContactAsync(SupplierContactUpdateDto dto)
		{
			var entity = _mapper.Map<SupplierContact>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateContactAsync(entity);
			return updated == null ? null : _mapper.Map<SupplierContactResponseDto>(updated);
		}

		// Get compliance documents by supplier
		public Task<List<ComplianceDoc>> GetComplianceDocsAsync(long supplierId) =>
			_repo.GetComplianceDocsBySupplierAsync(supplierId);

		// Add compliance document to supplier
		public async Task<ComplianceDoc> AddComplianceDocAsync(ComplianceDoc doc)
		{
			doc.CreatedOn = DateTime.UtcNow;
			doc.UpdatedOn = DateTime.UtcNow;
			doc.Status = "ACTIVE";
			doc.IsDeleted = false;
			return await _repo.AddComplianceDocAsync(doc);
		}

		// Update compliance document
		public async Task<ComplianceDocResponseDto?> UpdateComplianceDocAsync(ComplianceDocUpdateDto dto)
		{
			var entity = _mapper.Map<ComplianceDoc>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateComplianceDocAsync(entity);
			return updated == null ? null : _mapper.Map<ComplianceDocResponseDto>(updated);
		}

		// Get risks by supplier
		public Task<List<SupplierRisk>> GetRisksAsync(long supplierId) =>
			_repo.GetRisksBySupplierAsync(supplierId);

		// Add risk to supplier
		public async Task<SupplierRisk> AddRiskAsync(SupplierRisk risk)
		{
			risk.CreatedOn = DateTime.UtcNow;
			risk.UpdatedOn = DateTime.UtcNow;
			risk.Status = "ACTIVE";
			risk.IsDeleted = false;
			return await _repo.AddRiskAsync(risk);
		}

		// Update supplier risk
		public async Task<SupplierRiskResponseDto?> UpdateRiskAsync(SupplierRiskUpdateDto dto)
		{
			var entity = _mapper.Map<SupplierRisk>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateRiskAsync(entity);
			return updated == null ? null : _mapper.Map<SupplierRiskResponseDto>(updated);
		}

		// Get all organizations
		public Task<List<Organization>> GetOrganizationsAsync() =>
			_repo.GetAllOrganizationsAsync();

		// Add organization
		public async Task<Organization> AddOrganizationAsync(Organization org)
		{
			org.CreatedOn = DateTime.UtcNow;
			org.UpdatedOn = DateTime.UtcNow;
			org.Status = "ACTIVE";
			org.IsDeleted = false;
			return await _repo.AddOrganizationAsync(org);
		}

		// Update organization
		public async Task<OrganizationResponseDto?> UpdateOrganizationAsync(OrganizationUpdateDto dto)
		{
			var entity = _mapper.Map<Organization>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateOrganizationAsync(entity);
			return updated == null ? null : _mapper.Map<OrganizationResponseDto>(updated);
		}
	}
}