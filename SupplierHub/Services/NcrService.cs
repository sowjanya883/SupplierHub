using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;
using SupplierHub.DTOs.NcrDTO;

namespace SupplierHub.Services
{
    public class NcrService : INcrService
    {
        private readonly INcrRepository _repo;
        private readonly IMapper _mapper;
		private readonly INotificationService _notif;

		public NcrService(INcrRepository repo, IMapper mapper, INotificationService notif)
        {
            _repo = repo;
            _mapper = mapper;
            _notif = notif;
        }

        public async Task<NcrReadDto> CreateNcrAsync(NcrCreateDto dto)
        {
            var entity = _mapper.Map<Ncr>(dto);
            var saved = await _repo.AddNcrAsync(entity);
			var severity = saved.Severity ?? "Unknown";
			var msg = $"NCR-{saved.NcrID} raised: {severity} severity defect" +(string.IsNullOrEmpty(saved.DefectType)? ".": $" — {saved.DefectType}.");

			await _notif.SendToRoleAsync("Admin", msg, "NCR", saved.NcrID);
			await _notif.SendToRoleAsync("Buyer", msg, "NCR", saved.NcrID);

			if (severity == "Critical")
				await _notif.SendToRoleAsync("ReceivingUser",$"CRITICAL NCR-{saved.NcrID} raised. Immediate disposition required.","NCR", saved.NcrID);
			return _mapper.Map<NcrReadDto>(saved);
        }

        public async Task<List<NcrReadDto>> GetAllNcrsAsync() =>
            _mapper.Map<List<NcrReadDto>>(await _repo.GetAllNcrsAsync());

        public async Task<NcrReadDto?> GetNcrByIdAsync(long id)
        {
            var entity = await _repo.GetNcrByIdAsync(id);
            return entity == null ? null : _mapper.Map<NcrReadDto>(entity);
        }

        public async Task<List<NcrReadDto>> GetNcrsByItemIdAsync(long grnItemId) =>
            _mapper.Map<List<NcrReadDto>>(await _repo.GetNcrsByItemIdAsync(grnItemId));

        public async Task<NcrReadDto?> UpdateNcrAsync(long id, NcrUpdateDto dto)
        {
            var existing = await _repo.GetNcrByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateNcrAsync(existing);
            return _mapper.Map<NcrReadDto>(updated);
        }

        public async Task<bool> DeleteNcrAsync(long id) => await _repo.DeleteNcrAsync(id);
    }
}