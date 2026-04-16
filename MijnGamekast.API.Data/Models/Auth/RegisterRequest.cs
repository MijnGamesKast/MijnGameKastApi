using System.ComponentModel.DataAnnotations;

namespace MijnGameKast.API.Data.Models.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Vul een gebruikersnaam in.")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Vul een e-mailadres in.")]
    [EmailAddress(ErrorMessage = "Vul een geldig e-mailadres in.")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Vul een wachtwoord in.")]
    public string Password { get; set; } = string.Empty;
}