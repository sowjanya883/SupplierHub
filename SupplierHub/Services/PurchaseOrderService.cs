using AutoMapper;
using SupplierHub.DTOs.PurchaseOrderDTO;
using SupplierHub.Models;
using SupplierHub.Repositories; // Assuming your interface is here
using SupplierHub.Repositories.Interface; // Assuming your interface is here
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class PurchaseOrderService : IPurchaseOrderService
	{
		private readonly IPurchaseOrderRepository _repository;
		private readonly IMapper _mapper;

		public PurchaseOrderService(IPurchaseOrderRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<PurchaseOrderResponseDto>> GetAllAsync()
		{
			// The Repository handles the "!IsDeleted" filter logic
			var orders = await _repository.GetAllActiveAsync();
			return _mapper.Map<IEnumerable<PurchaseOrderResponseDto>>(orders);
		}

		public async Task<PurchaseOrderResponseDto?> GetByIdAsync(long id)
		{
			var order = await _repository.GetByIdAsync(id);
			if (order == null) return null;

			return _mapper.Map<PurchaseOrderResponseDto>(order);
		}

		public async Task<PurchaseOrderResponseDto> CreateAsync(PurchaseOrderCreateDto createDto)
		{
			var order = _mapper.Map<PurchaseOrder>(createDto);

			// Business Logic: Setting mandatory audit fields
			order.CreatedOn = DateTime.UtcNow;
			order.UpdatedOn = DateTime.UtcNow;
			order.IsDeleted = false;

			// Optional: Default status if not provided
			if (string.IsNullOrEmpty(order.Status))
			{
				order.Status = "Open";
			}

			await _repository.AddAsync(order);
			await _repository.SaveChangesAsync();

			return _mapper.Map<PurchaseOrderResponseDto>(order);
		}

		public async Task<PurchaseOrderResponseDto?> UpdateAsync(long id, PurchaseOrderUpdateDto updateDto)
		{
			var existingOrder = await _repository.GetByIdAsync(id);

			// Validation: Check if exists and isn't soft-deleted
			if (existingOrder == null) return null;

			// AutoMapper updates existingOrder with values from updateDto
			_mapper.Map(updateDto, existingOrder);

			// Business Logic: Update the timestamp
			existingOrder.UpdatedOn = DateTime.UtcNow;

			_repository.Update(existingOrder);
			await _repository.SaveChangesAsync();

			return _mapper.Map<PurchaseOrderResponseDto>(existingOrder);
		}

		public async Task<bool> DeleteAsync(long id)
		{
			var order = await _repository.GetByIdAsync(id);
			if (order == null) return false;

			// Soft Delete Implementation
			order.IsDeleted = true;
			order.UpdatedOn = DateTime.UtcNow;

			_repository.Update(order);
			await _repository.SaveChangesAsync();

			return true;
		}
	}
}