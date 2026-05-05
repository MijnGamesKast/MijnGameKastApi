namespace MijnGameKast.API.Data.Models.Auth;

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
}