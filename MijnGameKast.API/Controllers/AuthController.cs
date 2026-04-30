using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Data.Models.Auth;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
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

    /// <summary>
    /// Logt een gebruiker in en geneert een token
    /// </summary>
    /// <param name="request">Login gegevens (email/username + wachtwoord)</param>
    /// <returns>Token en gebruikersinformatie</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
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

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = GetBearerToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven"
            });
        }

        var success = await _authService.LogoutAsync(token);

        if (!success)
        {
            return NotFound(new
            {
                Message = "Geen actieve sessie gevonden voor deze token"
            });
        }

        return Ok(new
        {
            Message = "Succesvol uitgelogd"
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var token = GetBearerToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven"
            });
        }

        var result = await _authService.GetMeAsync(token);

        if (result == null)
        {
            return Unauthorized(new
            {
                Message = "Geen geldige ingelogde gamer gevonden"
            });
        }
        
        return Ok(result);
    }
    
    private Dictionary<string, List<string>> GetValidationErrors()
    {
        return ModelState
            .Where(x => x.Value is not null && x.Value.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
            );
    }

    private string? GetBearerToken()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return null;
        }

        const string bearerPrefix = "Bearer ";

        if (!authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return authorizationHeader[bearerPrefix.Length..].Trim();
    }
}