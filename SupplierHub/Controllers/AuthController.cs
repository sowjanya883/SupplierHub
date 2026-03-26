// /SupplierHub/Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupplierHub.DTOs.UserDTO;
using SupplierHub.Services.Interface;
using System;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[AllowAnonymous]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly ILogger<AuthController> _logger;

		public AuthController(
			IAuthService authService,
			ILogger<AuthController> logger)
		{
			_authService = authService;
			_logger = logger;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
		{
			if (!ModelState.IsValid)
				return ValidationProblem(ModelState);

			try
			{
				var result = await _authService.LoginAsync(dto);
				return Ok(result);
			}
			catch (UnauthorizedAccessException ex)
			{
				// ✅ EXPECTED authentication failure
				_logger.LogWarning(ex, "Login failed for email {Email}", dto.Email);
				return Unauthorized(new
				{
					message = ex.Message
				});
			}
			catch (Exception ex)
			{
				// ✅ UNEXPECTED errors
				_logger.LogError(ex, "Unexpected error during login");
				return StatusCode(500, "An unexpected error occurred.");
			}
		}
	}
}
