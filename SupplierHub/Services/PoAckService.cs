using AutoMapper;
using SupplierHub.Constants.Enum;
using SupplierHub.DTOs.PoAckDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class PoAckService : IPoAckService
	{
		private readonly IPoAckRepository _repository;
		private readonly IPurchaseOrderRepository _poRepository;
		private readonly IMapper _mapper;
		private readonly INotificationService _notif;

		public PoAckService(
			IPoAckRepository repository,
			IPurchaseOrderRepository poRepository,
			IMapper mapper,
			INotificationService notif)
		{
			_repository = repository;
			_poRepository = poRepository;
			_mapper = mapper;
			_notif = notif;
		}

		// Maps the supplier's decision string (mapped from PoAckDecision enum) to the
		// PO header status the buyer should see.
		private static string? PoStatusFromDecision(string? decision)
		{
			if (string.IsNullOrWhiteSpace(decision)) return null;
			if (decision.Equals("Accept",  StringComparison.OrdinalIgnoreCase)) return "Acknowledged";
			if (decision.Equals("Counter", StringComparison.OrdinalIgnoreCase)) return "Counter";
			if (decision.Equals("Decline", StringComparison.OrdinalIgnoreCase)) return "Declined";
			return null;
		}

		public async Task<IEnumerable<PoAckResponseDto>> GetAllAsync()
		{
			var acks = await _repository.GetAllActiveAsync();
			return _mapper.Map<IEnumerable<PoAckResponseDto>>(acks);
		}

		public async Task<PoAckResponseDto?> GetByIdAsync(long id)
		{
			var ack = await _repository.GetByIdAsync(id);
			return ack == null ? null : _mapper.Map<PoAckResponseDto>(ack);
		}

		public async Task<PoAckResponseDto> CreateAsync(PoAckCreateDto createDto)
		{
			// Business Rule: Require notes if countering
			if (createDto.Decision == PoAckDecision.Counter &&
				string.IsNullOrWhiteSpace(createDto.CounterNotes))
			{
				throw new ArgumentException(
					"Counter notes are strictly required when countering a Purchase Order.");
			}

			var ack = _mapper.Map<PoAck>(createDto);

			ack.CreatedOn = DateTime.UtcNow;
			ack.UpdatedOn = DateTime.UtcNow;
			ack.IsDeleted = false;

			if (string.IsNullOrWhiteSpace(ack.Status))
				ack.Status = nameof(PoAckStatus.Active);

			await _repository.AddAsync(ack);
			await _repository.SaveChangesAsync();

			// ── Auto-flip PO status to reflect the supplier's decision ──
			var newStatus = PoStatusFromDecision(ack.Decision);
			if (!string.IsNullOrEmpty(newStatus))
			{
				var po = await _poRepository.GetByIdAsync(ack.PoID);
				if (po != null && !string.Equals(po.Status, newStatus, StringComparison.OrdinalIgnoreCase))
				{
					po.Status = newStatus;
					po.UpdatedOn = DateTime.UtcNow;
					_poRepository.Update(po);
					await _poRepository.SaveChangesAsync();
				}
			}

			// ── Notifications ──────────────────────────
			var decision = ack.Decision?.ToString() ?? "responded";

			await _notif.SendToRoleAsync(
				"Buyer",
				$"Supplier has {decision} PO-{ack.PoID}. Please review the acknowledgement.",
				"PurchaseOrder",
				ack.PoID);

			await _notif.SendToRoleAsync(
				"Admin",
				$"PO-{ack.PoID} acknowledged by supplier with decision: {decision}.",
				"PurchaseOrder",
				ack.PoID);

			return _mapper.Map<PoAckResponseDto>(ack);
		}

		public async Task<PoAckResponseDto?> UpdateAsync(long id, PoAckUpdateDto updateDto)
		{
			var existingAck = await _repository.GetByIdAsync(id);
			if (existingAck == null) return null;

			if (updateDto.Decision == PoAckDecision.Counter &&
				string.IsNullOrWhiteSpace(updateDto.CounterNotes))
			{
				throw new ArgumentException(
					"Counter notes are strictly required when countering a Purchase Order.");
			}

			_mapper.Map(updateDto, existingAck);
			existingAck.UpdatedOn = DateTime.UtcNow;

			_repository.Update(existingAck);
			await _repository.SaveChangesAsync();

			return _mapper.Map<PoAckResponseDto>(existingAck);
		}
	}
}