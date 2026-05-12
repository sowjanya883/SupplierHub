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
		private readonly INotificationService _notif;

		public RfxService(
			IRfxRepository repo,
			IMapper mapper,
			INotificationService notif)
		{
			_repo = repo;
			_mapper = mapper;
			_notif = notif;
		}

		public async Task<RFxEventReadDto?> GetRfxByIdAsync(long id)
		{
			var entity = await _repo.GetRfxByIdAsync(id);
			return entity == null ? null : _mapper.Map<RFxEventReadDto>(entity);
		}

		public async Task<List<RFxEventReadDto>> GetAllRfxAsync()
		{
			var entities = await _repo.GetAllRfxAsync();
			return _mapper.Map<List<RFxEventReadDto>>(entities);
		}

		public async Task<RFxEventReadDto> CreateRfxAsync(RFxEventCreateDto dto)
		{
			var entity = _mapper.Map<RfxEvent>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddRfxAsync(entity);

			// Notify Buyers and CategoryManagers
			var msg = $"📢 New {saved.Type} event created: '{saved.Title}'. Open for review.";
			await _notif.SendToRoleAsync("Buyer", msg, "RFx", saved.RfxID);
			await _notif.SendToRoleAsync("CategoryManager", msg, "RFx", saved.RfxID);
			await _notif.SendToRoleAsync("Admin", msg, "RFx", saved.RfxID);

			return _mapper.Map<RFxEventReadDto>(saved);
		}

		public async Task<RFxEventReadDto?> UpdateRfxAsync(RFxEventUpdateDto dto)
		{
			var existing = await _repo.GetRfxByIdAsync(dto.RfxID);
			if (existing == null) return null;

			var oldStatus = existing.Status;
			_mapper.Map(dto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateRfxAsync(existing);
			if (updated == null) return null;

			// Notify if status changed (e.g. Closed, Awarded)
			if (updated.Status != oldStatus)
			{
				await _notif.SendToRoleAsync(
					"Buyer",
					$"📋 RFx event '{updated.Title}' status changed to {updated.Status}.",
					"RFx", updated.RfxID);
			}

			return _mapper.Map<RFxEventReadDto>(updated);
		}

		public async Task<List<RfxLineReadDto>> GetLinesByRfxAsync(long rfxId)
		{
			var entities = await _repo.GetLinesByRfxAsync(rfxId);
			return _mapper.Map<List<RfxLineReadDto>>(entities);
		}

		public async Task<RfxLineReadDto> AddRfxLineAsync(RfxLineCreateDto dto)
		{
			var entity = _mapper.Map<RfxLine>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddRfxLineAsync(entity);
			return _mapper.Map<RfxLineReadDto>(saved);
		}

		public async Task<RfxLineReadDto?> UpdateRfxLineAsync(RfxLineUpdateDto dto)
		{
			var existing = await _repo.GetRfxLineByIdAsync(dto.RfxLineID);
			if (existing == null) return null;

			_mapper.Map(dto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateRfxLineAsync(existing);
			return updated == null ? null : _mapper.Map<RfxLineReadDto>(updated);
		}

		public async Task<List<RfxInviteReadDto>> GetInvitesByRfxAsync(long rfxId)
		{
			var entities = await _repo.GetInvitesByRfxAsync(rfxId);
			return _mapper.Map<List<RfxInviteReadDto>>(entities);
		}

		public async Task<RfxInviteReadDto> AddInviteAsync(RfxInviteCreateDto dto)
		{
			var entity = _mapper.Map<RfxInvite>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddInviteAsync(entity);

			// Notify all SupplierUsers about the invite
			await _notif.SendToRoleAsync(
				"SupplierUser",
				$"📩 You have been invited to participate in RFx event #{dto.RfxID}. " +
				$"Please review the requirements and submit your bid before the closing date.",
				"RFx",
				dto.RfxID);

			return _mapper.Map<RfxInviteReadDto>(saved);
		}

		public async Task<RfxInviteReadDto?> UpdateInviteAsync(RfxInviteUpdateDto dto)
		{
			// Load the existing invite so required fields (RfxID, SupplierID, CreatedOn) aren't wiped.
			var existing = await _repo.GetInviteByIdAsync(dto.InviteID);
			if (existing == null) return null;

			if (!string.IsNullOrWhiteSpace(dto.Status)) existing.Status = dto.Status;
			if (dto.InvitedDate.HasValue) existing.InvitedDate = dto.InvitedDate;
			if (dto.IsDeleted.HasValue) existing.IsDeleted = dto.IsDeleted.Value;
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateInviteAsync(existing);
			if (updated == null) return null;

			// Notify Buyer / CategoryManager / Admin when supplier responds.
			if (!string.IsNullOrWhiteSpace(updated.Status) &&
				(updated.Status.Equals("Accepted", StringComparison.OrdinalIgnoreCase) ||
				 updated.Status.Equals("Declined", StringComparison.OrdinalIgnoreCase)))
			{
				var msg = $"Supplier #{updated.SupplierID} has {updated.Status.ToLower()} the invitation to RFx-{updated.RfxID}.";
				await _notif.SendToRoleAsync("Buyer", msg, "RFx", updated.RfxID);
				await _notif.SendToRoleAsync("CategoryManager", msg, "RFx", updated.RfxID);
				await _notif.SendToRoleAsync("Admin", msg, "RFx", updated.RfxID);
			}

			return _mapper.Map<RfxInviteReadDto>(updated);
		}

		public async Task<List<BidReadDto>> GetBidsByRfxAsync(long rfxId)
		{
			var entities = await _repo.GetBidsByRfxAsync(rfxId);
			return _mapper.Map<List<BidReadDto>>(entities);
		}

		public async Task<BidReadDto?> GetBidByIdAsync(long bidId)
		{
			var entity = await _repo.GetBidByIdAsync(bidId);
			return entity == null ? null : _mapper.Map<BidReadDto>(entity);
		}

		public async Task<BidReadDto> AddBidAsync(BidCreateDto dto)
		{
			var entity = _mapper.Map<Bid>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddBidAsync(entity);

			// Notify Buyer and CategoryManager that a bid was submitted
			var msg = $"💼 Supplier #{saved.SupplierID} submitted a bid for RFx #{saved.RfxID}. " +
					  $"Total value: {saved.TotalValue?.ToString("N2") ?? "—"} {saved.Currency}.";
			await _notif.SendToRoleAsync("Buyer", msg, "RFx", saved.RfxID);
			await _notif.SendToRoleAsync("CategoryManager", msg, "RFx", saved.RfxID);
			await _notif.SendToRoleAsync("Admin", msg, "RFx", saved.RfxID);

			return _mapper.Map<BidReadDto>(saved);
		}

		public async Task<BidReadDto?> UpdateBidAsync(BidUpdateDto dto)
		{
			var entity = _mapper.Map<Bid>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateBidAsync(entity);
			return updated == null ? null : _mapper.Map<BidReadDto>(updated);
		}

		public async Task<List<BidLineReadDto>> GetBidLinesByBidAsync(long bidId)
		{
			var entities = await _repo.GetBidLinesByBidAsync(bidId);
			return _mapper.Map<List<BidLineReadDto>>(entities);
		}

		public async Task<BidLineReadDto> AddBidLineAsync(BidLineCreateDto dto)
		{
			var entity = _mapper.Map<BidLine>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddBidLineAsync(entity);
			return _mapper.Map<BidLineReadDto>(saved);
		}

		public async Task<BidLineReadDto?> UpdateBidLineAsync(BidLineUpdateDto dto)
		{
			var entity = _mapper.Map<BidLine>(dto);
			entity.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateBidLineAsync(entity);
			return updated == null ? null : _mapper.Map<BidLineReadDto>(updated);
		}

		public async Task<List<AwardReadDto>> GetAwardsByRfxAsync(long rfxId)
		{
			var entities = await _repo.GetAwardsByRfxAsync(rfxId);
			return _mapper.Map<List<AwardReadDto>>(entities);
		}
		public async Task<AwardReadDto> AddAwardAsync(AwardCreateDto dto)
		{
			var entity = _mapper.Map<Award>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;
			entity.IsDeleted = false;

			var saved = await _repo.AddAwardAsync(entity);

			// Notify winning supplier
			await _notif.SendToRoleAsync(
				"SupplierUser",
				$"🏆 Congratulations! Your bid for RFx #{saved.RfxID} has been awarded. " +
				$"Award value: {saved.AwardValue?.ToString("N2") ?? "—"} {saved.Currency}. " +
				$"Please check your purchase orders.",
				"RFx",
				saved.RfxID);

			// Notify buyers
			await _notif.SendToRoleAsync(
				"Buyer",
				$"✅ RFx #{saved.RfxID} has been awarded to Supplier #{saved.SupplierID}. " +
				$"Award value: {saved.AwardValue?.ToString("N2") ?? "—"} {saved.Currency}.",
				"RFx",
				saved.RfxID);

			await _notif.SendToRoleAsync(
				"Admin",
				$"✅ RFx #{saved.RfxID} awarded to Supplier #{saved.SupplierID}.",
				"RFx",
				saved.RfxID);

			return _mapper.Map<AwardReadDto>(saved);
		}

		public async Task<AwardReadDto?> UpdateAwardAsync(AwardUpdateDto dto)
		{
			// Load existing so RfxID, SupplierID, CreatedOn aren't wiped to defaults.
			var existing = await _repo.GetAwardByIdAsync(dto.AwardID);
			if (existing == null) return null;

			if (dto.AwardDate.HasValue) existing.AwardDate = dto.AwardDate;
			if (dto.AwardValue.HasValue) existing.AwardValue = dto.AwardValue;
			if (!string.IsNullOrWhiteSpace(dto.Currency)) existing.Currency = dto.Currency;
			if (dto.Notes != null) existing.Notes = dto.Notes;
			if (!string.IsNullOrWhiteSpace(dto.Status)) existing.Status = dto.Status;
			if (dto.IsDeleted.HasValue) existing.IsDeleted = dto.IsDeleted.Value;
			existing.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAwardAsync(existing);
			if (updated == null) return null;

			// Notify roles when award status changes (e.g. Awarded → Cancelled)
			if (!string.IsNullOrWhiteSpace(dto.Status))
			{
				var msg = $"Award #{updated.AwardID} for RFx-{updated.RfxID} updated to '{updated.Status}'.";
				await _notif.SendToRoleAsync("SupplierUser", msg, "RFx", updated.RfxID);
				await _notif.SendToRoleAsync("Buyer", msg, "RFx", updated.RfxID);
				await _notif.SendToRoleAsync("Admin", msg, "RFx", updated.RfxID);
			}

			return _mapper.Map<AwardReadDto>(updated);
		}
	}
}