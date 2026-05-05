using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Auth;

namespace MijnGameKast.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<bool> LogoutAsync(Session session);
    Task<AuthResult?> GetMeAsync(User user, Session session);
}