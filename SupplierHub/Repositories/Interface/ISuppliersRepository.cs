using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface ISuppliersRepository
	{
		// Supplier
		Task<Supplier?> GetSupplierByIdAsync(long id);
		Task<List<Supplier>> GetAllSuppliersAsync();
		Task<Supplier> AddSupplierAsync(Supplier supplier);
		Task<Supplier?> UpdateSupplierAsync(Supplier supplier);

		// SupplierContact
		Task<List<SupplierContact>> GetContactsBySupplierAsync(long supplierId);
		Task<SupplierContact> AddContactAsync(SupplierContact contact);
		Task<SupplierContact?> UpdateContactAsync(SupplierContact contact);

		// ComplianceDoc
		Task<List<ComplianceDoc>> GetComplianceDocsBySupplierAsync(long supplierId);
		Task<ComplianceDoc> AddComplianceDocAsync(ComplianceDoc doc);
		Task<ComplianceDoc?> UpdateComplianceDocAsync(ComplianceDoc doc);

		// SupplierRisk
		Task<List<SupplierRisk>> GetRisksBySupplierAsync(long supplierId);
		Task<SupplierRisk> AddRiskAsync(SupplierRisk risk);
		Task<SupplierRisk?> UpdateRiskAsync(SupplierRisk risk);

		// Organization
		Task<List<Organization>> GetAllOrganizationsAsync();
		Task<Organization> AddOrganizationAsync(Organization org);
		Task<Organization?> UpdateOrganizationAsync(Organization org);
	}
}