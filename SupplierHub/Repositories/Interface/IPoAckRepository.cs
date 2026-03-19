using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
    public interface IPoAckRepository
    {
        Task<IEnumerable<PoAck>> GetAllActiveAsync();
        Task<PoAck?> GetByIdAsync(long id);
        Task AddAsync(PoAck poAck);
        void Update(PoAck poAck);
        Task SaveChangesAsync();
    }
}
