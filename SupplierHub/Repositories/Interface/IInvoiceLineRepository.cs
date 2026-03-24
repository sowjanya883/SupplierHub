using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IInvoiceLineRepository
	{
		Task<IEnumerable<InvoiceLine>> GetByInvoiceIdAsync(long invoiceId);
		Task<InvoiceLine?> GetByIdAsync(long id);
		Task AddAsync(InvoiceLine line);
		void Update(InvoiceLine line);
		Task SaveChangesAsync();
	}
}