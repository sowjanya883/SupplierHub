using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SupplierHub.DTOs.UserDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _repo;
		private readonly IMapper _mapper;
		private readonly IPasswordHasher<User> _passwordHasher;

		public UserService(
			IUserRepository repo,
			IMapper mapper,
			IPasswordHasher<User> passwordHasher)
		{
			_repo = repo;
			_mapper = mapper;
			_passwordHasher = passwordHasher;
		}

		public async Task<List<UserDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
		{
			var users = await _repo.GetAllAsync(includeDeleted, ct);
			return _mapper.Map<List<UserDto>>(users);
		}

		public async Task<UserDto?> GetByIdAsync(long id, CancellationToken ct = default)
		{
			var user = await _repo.GetByIdAsync(id, false, ct);
			if (user == null) return null;
			return _mapper.Map<UserDto>(user);
		}

		public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
		{
			if(string.IsNullOrWhiteSpace(dto.Email))

	{
				throw new System.ArgumentException("Email is required.", nameof(dto.Email));
			}

			// 2. Now the compiler knows dto.Email is not null here
			var existingUser = await _repo.GetByEmailAsync(dto.Email, true, ct);
			if (existingUser != null)
			{
				throw new System.Exception("Email is already registered.");
			}

			var user = _mapper.Map<User>(dto);

			// ✅ HASH PASSWORD (CRITICAL FIX)
			user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

			user.Email = user.Email.ToLowerInvariant();
			user.CreatedOn = System.DateTime.UtcNow;
			user.UpdatedOn = user.CreatedOn;
			user.IsDeleted = false;

			await _repo.AddAsync(user, ct);
			await _repo.SaveChangesAsync(ct);

			return _mapper.Map<UserDto>(user);
		}

		public async Task<UserDto?> UpdateAsync(long id, UpdateUserDto dto, CancellationToken ct = default)
		{
			var user = await _repo.GetByIdAsync(id, true, ct);
			if (user == null) return null;

			if (dto.OrgID.HasValue) user.OrgID = dto.OrgID.Value;
			if (!string.IsNullOrWhiteSpace(dto.UserName)) user.UserName = dto.UserName;
			if (!string.IsNullOrWhiteSpace(dto.Email)) user.Email = dto.Email.ToLowerInvariant();
			if (!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;

			// HASH PASSWORD ON UPDATE
			if (!string.IsNullOrWhiteSpace(dto.Password))
			{
				user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
			}

			if (!string.IsNullOrWhiteSpace(dto.Status))
			{
				user.Status = dto.Status;
				// Reactivating: setting Status=Active on a soft-deleted user clears the IsDeleted flag.
				if (string.Equals(dto.Status, "Active", System.StringComparison.OrdinalIgnoreCase))
					user.IsDeleted = false;
			}
			if (dto.IsDeleted.HasValue) user.IsDeleted = dto.IsDeleted.Value;

			user.UpdatedOn = System.DateTime.UtcNow;

			await _repo.UpdateAsync(user, ct);
			await _repo.SaveChangesAsync(ct);

			return _mapper.Map<UserDto>(user);
		}

		public async Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default)
		{
			var ok = await _repo.SoftDeleteAsync(id, ct);
			if (!ok) return false;
			await _repo.SaveChangesAsync(ct);
			return true;
		}

		public Task<bool> ExistsAsync(long id, CancellationToken ct = default)
		{
			return _repo.ExistsAsync(id, false, ct);
		}
	}
}