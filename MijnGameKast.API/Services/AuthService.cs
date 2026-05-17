using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Auth;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;

    public AuthService(IUserRepository userRepository, ISessionRepository sessionRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        // Check if Email is already in use
        var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
        // Check if Username is already in use
        var existingUserByUsername = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUserByUsername != null || existingUserByEmail != null)
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Een ingevuld veld is niet beschikbaar!"
            };
        }
        
        // Create user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddUserAsync(user);

        return new AuthResult
        {
            Success = true,
            Message = "Gamer succesvol aangemaakt!",
            UserId = createdUser.Id,
            Username = createdUser.Username,
            Email = createdUser.Email
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        User? user;

        // Check if the Identifier is an Email or Username
        if (request.Identifier.Contains("@"))
        {
            user = await _userRepository.GetByEmailAsync(request.Identifier);
        }
        else
        {
            user = await _userRepository.GetByUsernameAsync(request.Identifier);
        }
        
        // Check if User exists
        // Threat ID 15: Information leak using error messaging. Only show error message that login details are incorrect.
        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Ongeldige inloggegevens!"
            };
        }

        // Check if Password matches
        var passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordMatches)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Ongeldige inloggegevens!"
            };
        }
        
        // Create Session
        var session = new Session
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(6)
        };

        await _sessionRepository.AddAsync(session);

        return new AuthResult
        {
            Success = true,
            Message = "Succesvol ingelogd!",
            Token = session.Token,
            ExpiresAt = session.ExpiresAt,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<bool> LogoutAsync(Session session)
    {
        session.ExpiresAt = DateTime.UtcNow;
        return await _sessionRepository.UpdateAsync(session);
    }

    public async Task<AuthResult?> GetMeAsync(User user, Session session)
    {
        return new AuthResult
        {
            Success = true,
            Message = "Gebruiker succesvol gevonden!",
            Token = session.Token,
            ExpiresAt = session.ExpiresAt,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }
}