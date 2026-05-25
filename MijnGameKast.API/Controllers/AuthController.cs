using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Attributes;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Auth;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : CustomBaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "De registratiegegeven zijn ongeldig.",
                Errors = GetValidationErrors()
            });
        }

        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                Message = "Er is geen geldige loginrequest meegestuurd"
            });
        }
        
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "De ingevoerde logingegevens zijn ongeldig.",
                Errors = GetValidationErrors()
            });
        }

        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [RequireAuth]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var session = GetSession();
        await _authService.LogoutAsync(session);
        return Ok(new
        {
            Message = "Succesvol uitgelogd"
        });
    }

    [RequireAuth]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        User? user = GetUser();
        Session? session = GetSession();

        var result = await _authService.GetMeAsync(user, session);
        
        return Ok(result);
    }
}