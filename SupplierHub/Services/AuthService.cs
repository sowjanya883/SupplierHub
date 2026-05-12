using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupplierHub.DTOs.AuditLogDTO;
using SupplierHub.DTOs.NotificationDTO;
using SupplierHub.DTOs.UserDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepository;
		private readonly IPasswordHasher<User> _passwordHasher;
		private readonly IConfiguration _config;
		private readonly IAuditLogService _auditLogService;
		private readonly INotificationService _notificationService;

		public AuthService(
			IUserRepository userRepository,
			IPasswordHasher<User> passwordHasher,
			IConfiguration config,
			IAuditLogService auditLogService,
			INotificationService notificationService)
		{
			_userRepository = userRepository;
			_passwordHasher = passwordHasher;
			_config = config;
			_auditLogService = auditLogService;
			_notificationService = notificationService;
		}

		public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
		{
			if (dto == null)
				throw new ArgumentNullException(nameof(dto));

			var email = dto.Email.Trim().ToLowerInvariant();

			var user = await _userRepository.GetByEmailAsync(email)
				?? throw new UnauthorizedAccessException("Invalid credentials.");

			if (user.IsDeleted)
				throw new UnauthorizedAccessException("This account has been deactivated.");

			if (user.Status is "Inactive" or "Suspended" or "Pending")
				throw new UnauthorizedAccessException("User is not active.");

			var verifyResult = _passwordHasher.VerifyHashedPassword(
				user,
				user.PasswordHash ?? string.Empty,
				dto.Password);

			if (verifyResult == PasswordVerificationResult.Failed)
				throw new UnauthorizedAccessException("Invalid credentials.");

			// Audit Log
			await _auditLogService.CreateAsync(new CreateAuditLogDto
			{
				UserID = user.UserID,
				Action = "UserLoggedIn",
				Resource = $"User:{user.UserID}",
				Timestamp = DateTime.UtcNow,
				Status = "Success"
			});

			// Notification
			// Notify Admin that a user has logged in
			// Notify Admin on login
			var loginTime = DateTime.UtcNow.ToString("dd MMM yyyy, HH:mm");
			await _notificationService.SendToRoleAsync(
				"Admin",
				$"🔐 '{user.UserName}' ({user.Email}) logged in at {loginTime} UTC.",
				"System",
				user.UserID);

			// Roles
			var roles = await _userRepository
				.GetRoleNamesByUserIdAsync(user.UserID);

			// JWT
			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
			);

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var expiryMinutes = int.TryParse(
				_config["Jwt:ExpiryMinutes"],
				out var m) ? m : 60;

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
				new Claim(ClaimTypes.Name, user.UserName)
			};

			claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
				signingCredentials: creds
			);

			return new LoginResponseDto
			{
				Token = new JwtSecurityTokenHandler().WriteToken(token),
				ExpiresAtUtc = token.ValidTo,
				UserId = (int)user.UserID,
				Name = user.UserName,
				Email = user.Email,
				Roles = roles
			};
		}
	}
}