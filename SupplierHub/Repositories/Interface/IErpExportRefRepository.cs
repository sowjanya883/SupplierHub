using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IErpExportRefRepository
	{
		Task<IEnumerable<ErpExportRef>> GetAllActiveAsync();
		Task<ErpExportRef?> GetByIdAsync(long id);
		Task AddAsync(ErpExportRef erpExportRef);
		void Update(ErpExportRef erpExportRef);
		Task SaveChangesAsync();
	}
}