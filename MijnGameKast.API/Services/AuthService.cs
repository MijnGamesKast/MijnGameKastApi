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
        if (existingUserByEmail != null)
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Er bestaat al een gamer met dit e-mailadres! ({existingUserByEmail.Email})"
            };
        }
        
        // Check if Username is already in use
        var existingUserByUsername = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUserByUsername != null)
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Er bestaat al een gamer met de username ({existingUserByUsername.Username})"
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
            ExpiresAt = DateTime.UtcNow.AddHours(2)
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

    public async Task<bool> LogoutAsync(string token)
    {
        // Check if session exists
        var session = await _sessionRepository.GetByTokenAsync(token);
        if (session == null)
        {
            return false;
        }
        
        // Expire session
        session.ExpiresAt = DateTime.UtcNow;
        return await _sessionRepository.UpdateAsync(session);
    }

    public async Task<AuthResult?> GetMeAsync(string token)
    {
        // Check if a session exists
        var session = await _sessionRepository.GetByTokenAsync(token);
        // If a session does not exist, return null
        if (session == null)
        {
            return null;
        }

        // If a session is expired, return null
        if (session.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }
        
        var user = await _userRepository.GetByIdAsync(session.UserId);
        // If a user does not exist, return null
        if (user == null)
        {
            return null;
        }

        return new AuthResult
        {
            Success = true,
            Message = "Gebruiker succesvol gevonden!",
            Token = session.Token,
            ExpiresAt = session.ExpiresAt,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }
}