using MijnGameKast.API.Data.Models.Auth;

namespace MijnGameKast.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<bool> LogoutAsync(string token);
    Task<AuthResult?> GetMeAsync(string token);
}