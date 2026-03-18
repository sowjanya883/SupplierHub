using SupplierHub.DTOs.BidDTO;
using SupplierHub.DTOs.BidLineDTO;
using SupplierHub.DTOs.AwardDTO;
using SupplierHub.DTOs.RfxInviteDTO;
using SupplierHub.DTOs.RFxLineDTO;
using SupplierHub.DTOs.RfxEventDTO;
using SupplierHub.Models;

namespace SupplierHub.Services.Interface
{
	public interface IRfxService
	{
		// RfxEvent
		Task<RFxEventReadDto?> GetRfxByIdAsync(long id);
		Task<List<RFxEventReadDto>> GetAllRfxAsync();
		Task<RFxEventReadDto> CreateRfxAsync(RFxEventCreateDto dto);
		Task<RFxEventReadDto?> UpdateRfxAsync(RFxEventUpdateDto dto);

		// RfxLine
		Task<List<RfxLineReadDto>> GetLinesByRfxAsync(long rfxId);
		Task<RfxLineReadDto> AddRfxLineAsync(RfxLineCreateDto dto);
		Task<RfxLineReadDto?> UpdateRfxLineAsync(RfxLineUpdateDto dto);

		// RfxInvite
		Task<List<RfxInviteReadDto>> GetInvitesByRfxAsync(long rfxId);
		Task<RfxInviteReadDto> AddInviteAsync(RfxInviteCreateDto dto);
		Task<RfxInviteReadDto?> UpdateInviteAsync(RfxInviteUpdateDto dto);

		// Bid
		Task<List<BidReadDto>> GetBidsByRfxAsync(long rfxId);
		Task<BidReadDto?> GetBidByIdAsync(long bidId);
		Task<BidReadDto> AddBidAsync(BidCreateDto dto);
		Task<BidReadDto?> UpdateBidAsync(BidUpdateDto dto);

		// BidLine
		Task<List<BidLineReadDto>> GetBidLinesByBidAsync(long bidId);
		Task<BidLineReadDto> AddBidLineAsync(BidLineCreateDto dto);
		Task<BidLineReadDto?> UpdateBidLineAsync(BidLineUpdateDto dto);

		// Award
		Task<List<AwardReadDto>> GetAwardsByRfxAsync(long rfxId);
		Task<AwardReadDto> AddAwardAsync(AwardCreateDto dto);
		Task<AwardReadDto?> UpdateAwardAsync(AwardUpdateDto dto);
	}
}
