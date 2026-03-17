using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class SuppliersRepository : ISuppliersRepository
	{
		private readonly AppDbContext _db;

		public SuppliersRepository(AppDbContext db)
		{
			_db = db;
		}

		// Supplier
		public Task<Supplier?> GetSupplierByIdAsync(long id) =>
			_db.Suppliers.FirstOrDefaultAsync(x => x.SupplierID == id);

		public Task<List<Supplier>> GetAllSuppliersAsync() =>
			_db.Suppliers.ToListAsync();

		public async Task<Supplier> AddSupplierAsync(Supplier supplier)
		{
			_db.Suppliers.Add(supplier);
			await _db.SaveChangesAsync();
			return supplier;
		}

		public async Task<Supplier?> UpdateSupplierAsync(Supplier supplier)
		{
			_db.Suppliers.Update(supplier);
			await _db.SaveChangesAsync();
			return supplier;
		}

		// SupplierContact
		public Task<List<SupplierContact>> GetContactsBySupplierAsync(long supplierId) =>
			_db.SupplierContacts.Where(x => x.SupplierID == supplierId).ToListAsync();

		public async Task<SupplierContact> AddContactAsync(SupplierContact contact)
		{
			_db.SupplierContacts.Add(contact);
			await _db.SaveChangesAsync();
			return contact;
		}

		public async Task<SupplierContact?> UpdateContactAsync(SupplierContact contact)
		{
			_db.SupplierContacts.Update(contact);
			await _db.SaveChangesAsync();
			return contact;
		}

		// ComplianceDoc
		public Task<List<ComplianceDoc>> GetComplianceDocsBySupplierAsync(long supplierId) =>
			_db.ComplianceDocs.Where(x => x.SupplierID == supplierId).ToListAsync();

		public async Task<ComplianceDoc> AddComplianceDocAsync(ComplianceDoc doc)
		{
			_db.ComplianceDocs.Add(doc);
			await _db.SaveChangesAsync();
			return doc;
		}

		public async Task<ComplianceDoc?> UpdateComplianceDocAsync(ComplianceDoc doc)
		{
			_db.ComplianceDocs.Update(doc);
			await _db.SaveChangesAsync();
			return doc;
		}

		// SupplierRisk
		public Task<List<SupplierRisk>> GetRisksBySupplierAsync(long supplierId) =>
			_db.SupplierRisks.Where(x => x.SupplierID == supplierId).ToListAsync();

		public async Task<SupplierRisk> AddRiskAsync(SupplierRisk risk)
		{
			_db.SupplierRisks.Add(risk);
			await _db.SaveChangesAsync();
			return risk;
		}

		public async Task<SupplierRisk?> UpdateRiskAsync(SupplierRisk risk)
		{
			_db.SupplierRisks.Update(risk);
			await _db.SaveChangesAsync();
			return risk;
		}

		// Organization
		public Task<List<Organization>> GetAllOrganizationsAsync() =>
			_db.Organizations.ToListAsync();

		public async Task<Organization> AddOrganizationAsync(Organization org)
		{
			_db.Organizations.Add(org);
			await _db.SaveChangesAsync();
			return org;
		}

		public async Task<Organization?> UpdateOrganizationAsync(Organization org)
		{
			_db.Organizations.Update(org);
			await _db.SaveChangesAsync();
			return org;
		}
	}
}