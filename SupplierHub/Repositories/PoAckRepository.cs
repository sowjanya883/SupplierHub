using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class PoAckRepository : IPoAckRepository
	{
		private readonly AppDbContext _context;

		public PoAckRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<PoAck>> GetAllActiveAsync()
		{
			return await _context.PoAcks
				.Where(x => !x.IsDeleted)
				.ToListAsync();
		}

		public async Task<PoAck?> GetByIdAsync(long id)
		{
			return await _context.PoAcks
				.FirstOrDefaultAsync(x => x.PocfmID == id && !x.IsDeleted);
		}

		public async Task AddAsync(PoAck poAck) => await _context.PoAcks.AddAsync(poAck);

		public void Update(PoAck poAck) => _context.PoAcks.Update(poAck);

		public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
	}
}