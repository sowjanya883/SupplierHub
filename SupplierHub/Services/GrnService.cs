using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.GrnRefDTO;

namespace SupplierHub.Services
{
	public class GrnService : IGrnService
	{
		private readonly IGrnRepository _repo;
		private readonly IMapper _mapper;
		private readonly INotificationService _notif;

		public GrnService(
			IGrnRepository repo,
			IMapper mapper,
			INotificationService notif)
		{
			_repo = repo;
			_mapper = mapper;
			_notif = notif;
		}

		public async Task<GrnReadDto> CreateGrnAsync(GrnCreateDto dto)
		{
			var entity = _mapper.Map<GrnRef>(dto);
			var saved = await _repo.AddGrnAsync(entity);

			var msg = $"📦 GRN-{saved.GrnID} created for PO-{saved.PoID}. " +
					  $"Goods received on {DateTime.UtcNow:dd MMM yyyy} — awaiting inspection.";
			await _notif.SendToRoleAsync("Admin", msg, "GRN", saved.GrnID);
			await _notif.SendToRoleAsync("Buyer", msg, "GRN", saved.GrnID);
			await _notif.SendToRoleAsync("ReceivingUser", msg, "GRN", saved.GrnID);

			return _mapper.Map<GrnReadDto>(saved);
		}

		public async Task<List<GrnReadDto>> GetAllGrnsAsync() =>
			_mapper.Map<List<GrnReadDto>>(await _repo.GetAllGrnsAsync());

		public async Task<GrnReadDto?> GetGrnByIdAsync(long id)
		{
			var entity = await _repo.GetGrnByIdAsync(id);
			return entity == null ? null : _mapper.Map<GrnReadDto>(entity);
		}

		public async Task<GrnReadDto?> UpdateGrnAsync(long id, GrnUpdateDto dto)
		{
			var existing = await _repo.GetGrnByIdAsync(id);
			if (existing == null) return null;

			_mapper.Map(dto, existing);
			var updated = await _repo.UpdateGrnAsync(existing);
			return _mapper.Map<GrnReadDto>(updated);
		}

		public async Task<bool> DeleteGrnAsync(long id) =>
			await _repo.DeleteGrnAsync(id);
	}
}