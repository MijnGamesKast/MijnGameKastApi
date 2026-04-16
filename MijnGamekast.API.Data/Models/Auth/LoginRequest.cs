using System.ComponentModel.DataAnnotations;

namespace MijnGameKast.API.Data.Models.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Vul een gebruikersnaam of e-mailadres in.")]
    public string Identifier { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Vul een wachtwoord in.")]
    public string Password { get; set; } = string.Empty;
}