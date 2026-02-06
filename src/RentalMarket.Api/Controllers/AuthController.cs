using Microsoft.AspNetCore.Mvc;
using RentalMarket.Application.Auth;

namespace RentalMarket.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var token = await _authService.RegisterAsync(request.Email, request.Password, request.Username);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { Error = ex.Message });
        }
    }
}

// Simple DTOs (Data Transfer Objects) for the request
public record RegisterRequest(string Email, string Password, string Username);
public record LoginRequest(string Email, string Password);