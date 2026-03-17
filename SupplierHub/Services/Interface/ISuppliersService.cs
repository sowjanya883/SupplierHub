using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.DTOs.OrganizationDTO;
using SupplierHub.DTOs.SupplierContactDTO;
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.Models;

namespace SupplierHub.Services.Interface
{
	public interface ISuppliersService
	{
		// Supplier
		Task<SupplierResponseDto?> GetSupplierByIdAsync(long id);
		Task<List<SupplierListDto>> GetAllSuppliersAsync();
		Task<SupplierResponseDto> CreateSupplierAsync(SupplierCreateDto dto);
		Task<SupplierResponseDto?> UpdateSupplierAsync(SupplierUpdateDto dto);

		// SupplierContact
		Task<List<SupplierContact>> GetContactsAsync(long supplierId);
		Task<SupplierContact> AddContactAsync(SupplierContact contact);
		Task<SupplierContactResponseDto?> UpdateContactAsync(SupplierContactUpdateDto dto);

		// ComplianceDoc
		Task<List<ComplianceDoc>> GetComplianceDocsAsync(long supplierId);
		Task<ComplianceDoc> AddComplianceDocAsync(ComplianceDoc doc);
		Task<ComplianceDocResponseDto?> UpdateComplianceDocAsync(ComplianceDocUpdateDto dto);

		// SupplierRisk
		Task<List<SupplierRisk>> GetRisksAsync(long supplierId);
		Task<SupplierRisk> AddRiskAsync(SupplierRisk risk);
		Task<SupplierRiskResponseDto?> UpdateRiskAsync(SupplierRiskUpdateDto dto);

		// Organization
		Task<List<Organization>> GetOrganizationsAsync();
		Task<Organization> AddOrganizationAsync(Organization org);
		Task<OrganizationResponseDto?> UpdateOrganizationAsync(OrganizationUpdateDto dto);
	}
}