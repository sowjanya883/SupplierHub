using AutoMapper;
using SupplierHub.DTOs.BidDTO;
using SupplierHub.DTOs.BidLineDTO;
using SupplierHub.DTOs.AwardDTO;
using SupplierHub.DTOs.RfxInviteDTO;
using SupplierHub.DTOs.RFxLineDTO;
using SupplierHub.DTOs.RfxEventDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class RfxService : IRfxService
	{
		private readonly IRfxRepository _repo;
		private readonly IMapper _mapper;

		public RfxService(IRfxRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		/// <summary>
		/// Retrieves an RFx event by its ID.
		/// </summary>
		/// <param name="id">The RFx event ID</param>
		/// <returns>RFxEventReadDto if found; null otherwise</returns>
		public async Task<RFxEventReadDto?> GetRfxByIdAsync(long id)
		{
			var entity = await _repo.GetRfxByIdAsync(id);
			return entity == null ? null : _mapper.Map<RFxEventReadDto>(entity);
		}

		/// <summary>
		/// Retrieves all RFx events.
		/// </summary>
		/// <returns>List of RFxEventReadDto</returns>
		public async Task<List<RFxEventReadDto>> GetAllRfxAsync()
		{
			var entities = await _repo.GetAllRfxAsync();
			return _mapper.Map<List<RFxEventReadDto>>(entities);
		}

		/// <summary>
		/// Creates a new RFx event.
		/// </summary>
		/// <param name="dto">RFx event creation DTO</param>
		/// <returns>Created RFxEventReadDto</returns>
		public async Task<RFxEventReadDto> CreateRfxAsync(RFxEventCreateDto dto)
		{
			var entity = _mapper.Map<RfxEvent>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddRfxAsync(entity);
			return _mapper.Map<RFxEventReadDto>(saved);
		}

		/// <summary>
		/// Updates an existing RFx event.
		/// </summary>
		/// <param name="dto">RFx event update DTO</param>
		/// <returns>Updated RFxEventReadDto if found; null otherwise</returns>
		public async Task<RFxEventReadDto?> UpdateRfxAsync(RFxEventUpdateDto dto)
		{
			var existing = await _repo.GetRfxByIdAsync(dto.RfxID);
			if (existing == null) return null;

			_mapper.Map(dto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateRfxAsync(existing);
			return updated == null ? null : _mapper.Map<RFxEventReadDto>(updated);
		}

		/// <summary>
		/// Retrieves all RFx lines for a given RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of RfxLineReadDto</returns>
		public async Task<List<RfxLineReadDto>> GetLinesByRfxAsync(long rfxId)
		{
			var entities = await _repo.GetLinesByRfxAsync(rfxId);
			return _mapper.Map<List<RfxLineReadDto>>(entities);
		}

		/// <summary>
		/// Adds a new RFx line to an RFx event.
		/// </summary>
		/// <param name="dto">RFx line creation DTO</param>
		/// <returns>Created RfxLineReadDto</returns>
		public async Task<RfxLineReadDto> AddRfxLineAsync(RfxLineCreateDto dto)
		{
			var entity = _mapper.Map<RfxLine>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddRfxLineAsync(entity);
			return _mapper.Map<RfxLineReadDto>(saved);
		}

		/// <summary>
		/// Updates an existing RFx line.
		/// </summary>
		/// <param name="dto">RFx line update DTO</param>
		/// <returns>Updated RfxLineReadDto if found; null otherwise</returns>
		public async Task<RfxLineReadDto?> UpdateRfxLineAsync(RfxLineUpdateDto dto)
		{
			var existing = await _repo.GetRfxLineByIdAsync(dto.RfxLineID);
			if (existing == null) return null;

			_mapper.Map(dto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateRfxLineAsync(existing);
			return updated == null ? null : _mapper.Map<RfxLineReadDto>(updated);
		}

		/// <summary>
		/// Retrieves all RFx invites for a given RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of RfxInviteReadDto</returns>
		public async Task<List<RfxInviteReadDto>> GetInvitesByRfxAsync(long rfxId)
		{
			var entities = await _repo.GetInvitesByRfxAsync(rfxId);
			return _mapper.Map<List<RfxInviteReadDto>>(entities);
		}

		/// <summary>
		/// Adds a new RFx invite for a supplier.
		/// </summary>
		/// <param name="dto">RFx invite creation DTO</param>
		/// <returns>Created RfxInviteReadDto</returns>
		public async Task<RfxInviteReadDto> AddInviteAsync(RfxInviteCreateDto dto)
		{
			var entity = _mapper.Map<RfxInvite>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddInviteAsync(entity);
			return _mapper.Map<RfxInviteReadDto>(saved);
		}

		/// <summary>
		/// Updates an existing RFx invite status.
		/// </summary>
		/// <param >RFx invite update DTO</param>
		/// <returns>Updated RfxInviteReadDto if found; null otherwise</returns>
		public async Task<RfxInviteReadDto?> UpdateInviteAsync(RfxInviteUpdateDto dto)
		{
			var entity = _mapper.Map<RfxInvite>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateInviteAsync(entity);
			return updated == null ? null : _mapper.Map<RfxInviteReadDto>(updated);
		}

		/// <summary>
		/// Retrieves all bids for a given RFx event.
		/// </summary>
		/// <param >The RFx event ID</param>
		/// <returns>List of BidReadDto</returns>
		public async Task<List<BidReadDto>> GetBidsByRfxAsync(long rfxId)
		{
			var entities = await _repo.GetBidsByRfxAsync(rfxId);
			return _mapper.Map<List<BidReadDto>>(entities);
		}

		/// <summary>
		/// Retrieves a bid by its ID.
		/// </summary>
		/// <param >The bid ID</param>
		/// <returns>BidReadDto if found; null otherwise</returns>
		public async Task<BidReadDto?> GetBidByIdAsync(long bidId)
		{
			var entity = await _repo.GetBidByIdAsync(bidId);
			return entity == null ? null : _mapper.Map<BidReadDto>(entity);
		}

		/// <summary>
		/// Creates a new bid for an RFx event.
		/// </summary>
		/// <param >Bid creation DTO</param>
		/// <returns>Created BidReadDto</returns>
		public async Task<BidReadDto> AddBidAsync(BidCreateDto dto)
		{
			var entity = _mapper.Map<Bid>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddBidAsync(entity);
			return _mapper.Map<BidReadDto>(saved);
		}

		/// <summary>
		/// Updates an existing bid.
		/// </summary>
		/// <param >Bid update DTO</param>
		/// <returns>Updated BidReadDto if found; null otherwise</returns>
		public async Task<BidReadDto?> UpdateBidAsync(BidUpdateDto dto)
		{
			var entity = _mapper.Map<Bid>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateBidAsync(entity);
			return updated == null ? null : _mapper.Map<BidReadDto>(updated);
		}

		/// <summary>
		/// Retrieves all bid lines for a given bid.
		/// </summary>
		/// <param >The bid ID</param>
		/// <returns>List of BidLineReadDto</returns>
		public async Task<List<BidLineReadDto>> GetBidLinesByBidAsync(long bidId)
		{
			var entities = await _repo.GetBidLinesByBidAsync(bidId);
			return _mapper.Map<List<BidLineReadDto>>(entities);
		}

		/// <summary>
		/// Adds a new bid line to a bid.
		/// </summary>
		/// <param >Bid line creation DTO</param>
		/// <returns>Created BidLineReadDto</returns>
		public async Task<BidLineReadDto> AddBidLineAsync(BidLineCreateDto dto)
		{
			var entity = _mapper.Map<BidLine>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddBidLineAsync(entity);
			return _mapper.Map<BidLineReadDto>(saved);
		}

		/// <summary>
		/// Updates an existing bid line.
		/// </summary>
		/// <param >Bid line update DTO</param>
		/// <returns>Updated BidLineReadDto if found; null otherwise</returns>
		public async Task<BidLineReadDto?> UpdateBidLineAsync(BidLineUpdateDto dto)
		{
			var entity = _mapper.Map<BidLine>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateBidLineAsync(entity);
			return updated == null ? null : _mapper.Map<BidLineReadDto>(updated);
		}

		/// <summary>
		/// Retrieves all awards for a given RFx event.
		/// </summary>
		/// <param name="rfxId">The RFx event ID</param>
		/// <returns>List of AwardReadDto</returns>
		public async Task<List<AwardReadDto>> GetAwardsByRfxAsync(long rfxId)
		{
			var entities = await _repo.GetAwardsByRfxAsync(rfxId);
			return _mapper.Map<List<AwardReadDto>>(entities);
		}

		/// <summary>
		/// Creates a new award for an RFx event.
		/// </summary>
		/// <param >Award creation DTO</param>
		/// <returns>Created AwardReadDto</returns>
		public async Task<AwardReadDto> AddAwardAsync(AwardCreateDto dto)
		{
			var entity = _mapper.Map<Award>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddAwardAsync(entity);
			return _mapper.Map<AwardReadDto>(saved);
		}

		/// <summary>
		/// Updates an existing award.
		/// </summary>
		/// <param >Award update DTO</param>
		/// <returns>Updated AwardReadDto if found; null otherwise</returns>
		public async Task<AwardReadDto?> UpdateAwardAsync(AwardUpdateDto dto)
		{
			var entity = _mapper.Map<Award>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAwardAsync(entity);
			return updated == null ? null : _mapper.Map<AwardReadDto>(updated);
		}
	}
}
