using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        return Ok("User registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        throw new NotImplementedException();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        throw new NotImplementedException();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        throw new NotImplementedException();
    }
}