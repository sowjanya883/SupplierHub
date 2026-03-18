using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class RfxRepository : IRfxRepository
	{
		private readonly AppDbContext _db;

		public RfxRepository(AppDbContext db)
		{
			_db = db;
		}

		/// <summary>
		/// Retrieves an RFx event by its ID.
		/// </summary>
		/// <param name="id">The RFx event ID</param>
		/// <returns>RfxEvent if found; null otherwise</returns>
		public Task<RfxEvent?> GetRfxByIdAsync(long id) =>
			_db.RfxEvents.FirstOrDefaultAsync(x => x.RfxID == id);

		/// <summary>
		/// Retrieves all RFx events.
		/// </summary>
		/// <returns>List of RfxEvent</returns>
		public Task<List<RfxEvent>> GetAllRfxAsync() =>
			_db.RfxEvents.ToListAsync();

		/// <summary>
		/// Adds a new RFx event to the database.
		/// </summary>
		/// <param name="rfx">RfxEvent to add</param>
		/// <returns>Added RfxEvent</returns>
		public async Task<RfxEvent> AddRfxAsync(RfxEvent rfx)
		{
			_db.RfxEvents.Add(rfx);
			await _db.SaveChangesAsync();
			return rfx;
		}

		/// <summary>
		/// Updates an existing RFx event in the database.
		/// </summary>
		/// <param name="rfx">RfxEvent to update</param>
		/// <returns>Updated RfxEvent</returns>
		public async Task<RfxEvent?> UpdateRfxAsync(RfxEvent rfx)
		{
			_db.RfxEvents.Update(rfx);
			await _db.SaveChangesAsync();
			return rfx;
		}

		/// <summary>
		/// Retrieves an RFx line by its ID.
		/// </summary>
		/// <param name="lineId">The RFx line ID</param>
		/// <returns>RfxLine if found; null otherwise</returns>
		public Task<RfxLine?> GetRfxLineByIdAsync(long lineId) =>
			_db.RfxLines.FirstOrDefaultAsync(x => x.RfxLineID == lineId);

		/// <summary>
		/// Retrieves all RFx lines for a given RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of RfxLine</returns>
		public Task<List<RfxLine>> GetLinesByRfxAsync(long rfxId) =>
			_db.RfxLines.Where(x => x.RfxID == rfxId).ToListAsync();

		/// <summary>
		/// Adds a new RFx line to the database.
		/// </summary>
		/// <param name="line">RfxLine to add</param>
		/// <returns>Added RfxLine</returns>
		public async Task<RfxLine> AddRfxLineAsync(RfxLine line)
		{
			_db.RfxLines.Add(line);
			await _db.SaveChangesAsync();
			return line;
		}

		/// <summary>
		/// Updates an existing RFx line in the database.
		/// </summary>
		/// <param name="line">RfxLine to update</param>
		/// <returns>Updated RfxLine</returns>
		public async Task<RfxLine?> UpdateRfxLineAsync(RfxLine line)
		{
			_db.RfxLines.Update(line);
			await _db.SaveChangesAsync();
			return line;
		}

		/// <summary>
		/// Retrieves all RFx invites for a given RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of RfxInvite</returns>
		public Task<List<RfxInvite>> GetInvitesByRfxAsync(long rfxId) =>
			_db.RfxInvites.Where(x => x.RfxID == rfxId).ToListAsync();

		/// <summary>
		/// Adds a new RFx invite to the database.
		/// </summary>
		/// <param name="invite">RfxInvite to add</param>
		/// <returns>Added RfxInvite</returns>
		public async Task<RfxInvite> AddInviteAsync(RfxInvite invite)
		{
			_db.RfxInvites.Add(invite);
			await _db.SaveChangesAsync();
			return invite;
		}

		/// <summary>
		/// Updates an existing RFx invite in the database.
		/// </summary>
		/// <param name="invite">RfxInvite to update</param>
		/// <returns>Updated RfxInvite</returns>
		public async Task<RfxInvite?> UpdateInviteAsync(RfxInvite invite)
		{
			_db.RfxInvites.Update(invite);
			await _db.SaveChangesAsync();
			return invite;
		}

		/// <summary>
		/// Retrieves all bids for a given RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of Bid</returns>
		public Task<List<Bid>> GetBidsByRfxAsync(long rfxId) =>
			_db.Bids.Where(x => x.RfxID == rfxId).ToListAsync();

		/// <summary>
		/// Retrieves a bid by its ID.
		/// </summary>
		/// <param name="bidId">The bid ID</param>
		/// <returns>Bid if found; null otherwise</returns>
		public Task<Bid?> GetBidByIdAsync(long bidId) =>
			_db.Bids.FirstOrDefaultAsync(x => x.BidID == bidId);

		/// <summary>
		/// Adds a new bid to the database.
		/// </summary>
		/// <param name="bid">Bid to add</param>
		/// <returns>Added Bid</returns>
		public async Task<Bid> AddBidAsync(Bid bid)
		{
			_db.Bids.Add(bid);
			await _db.SaveChangesAsync();
			return bid;
		}

		/// <summary>
		/// Updates an existing bid in the database.
		/// </summary>
		/// <param name="bid">Bid to update</param>
		/// <returns>Updated Bid</returns>
		public async Task<Bid?> UpdateBidAsync(Bid bid)
		{
			_db.Bids.Update(bid);
			await _db.SaveChangesAsync();
			return bid;
		}

		/// <summary>
		/// Retrieves all bid lines for a given bid.
		/// </summary>
		/// <param name="bidId">The bid ID</param>
		/// <returns>List of BidLine</returns>
		public Task<List<BidLine>> GetBidLinesByBidAsync(long bidId) =>
			_db.BidLines.Where(x => x.BidID == bidId).ToListAsync();

		/// <summary>
		/// Adds a new bid line to the database.
		/// </summary>
		/// <param name="line">BidLine to add</param>
		/// <returns>Added BidLine</returns>
		public async Task<BidLine> AddBidLineAsync(BidLine line)
		{
			_db.BidLines.Add(line);
			await _db.SaveChangesAsync();
			return line;
		}

		/// <summary>
		/// Updates an existing bid line in the database.
		/// </summary>
		/// <param name="line">BidLine to update</param>
		/// <returns>Updated BidLine</returns>
		public async Task<BidLine?> UpdateBidLineAsync(BidLine line)
		{
			_db.BidLines.Update(line);
			await _db.SaveChangesAsync();
			return line;
		}

		/// <summary>
		/// Retrieves all awards for a given RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of Award</returns>
		public Task<List<Award>> GetAwardsByRfxAsync(long rfxId) =>
			_db.Awards.Where(x => x.RfxID == rfxId).ToListAsync();

		/// <summary>
		/// Adds a new award to the database.
		/// </summary>
		/// <param name="award">Award to add</param>
		/// <returns>Added Award</returns>
		public async Task<Award> AddAwardAsync(Award award)
		{
			_db.Awards.Add(award);
			await _db.SaveChangesAsync();
			return award;
		}

		/// <summary>
		/// Updates an existing award in the database.
		/// </summary>
		/// <param name="award">Award to update</param>
		/// <returns>Updated Award</returns>
		public async Task<Award?> UpdateAwardAsync(Award award)
		{
			_db.Awards.Update(award);
			await _db.SaveChangesAsync();
			return award;
		}
	}
}
