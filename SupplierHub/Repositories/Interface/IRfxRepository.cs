using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IRfxRepository
	{
		// RfxEvent
		Task<RfxEvent?> GetRfxByIdAsync(long id);
		Task<List<RfxEvent>> GetAllRfxAsync();
		Task<RfxEvent> AddRfxAsync(RfxEvent rfx);
		Task<RfxEvent?> UpdateRfxAsync(RfxEvent rfx);

		// RfxLine
		Task<RfxLine?> GetRfxLineByIdAsync(long lineId);
		Task<List<RfxLine>> GetLinesByRfxAsync(long rfxId);
		Task<RfxLine> AddRfxLineAsync(RfxLine line);
		Task<RfxLine?> UpdateRfxLineAsync(RfxLine line);

		// RfxInvite
		Task<List<RfxInvite>> GetInvitesByRfxAsync(long rfxId);
		Task<RfxInvite?> GetInviteByIdAsync(long inviteId);
		Task<RfxInvite> AddInviteAsync(RfxInvite invite);
		Task<RfxInvite?> UpdateInviteAsync(RfxInvite invite);

		// Bid
		Task<List<Bid>> GetBidsByRfxAsync(long rfxId);
		Task<Bid?> GetBidByIdAsync(long bidId);
		Task<Bid> AddBidAsync(Bid bid);
		Task<Bid?> UpdateBidAsync(Bid bid);

		// BidLine
		Task<List<BidLine>> GetBidLinesByBidAsync(long bidId);
		Task<BidLine> AddBidLineAsync(BidLine line);
		Task<BidLine?> UpdateBidLineAsync(BidLine line);

		// Award
		Task<List<Award>> GetAwardsByRfxAsync(long rfxId);
		Task<Award?> GetAwardByIdAsync(long awardId);
		Task<Award> AddAwardAsync(Award award);
		Task<Award?> UpdateAwardAsync(Award award);
	}
}
