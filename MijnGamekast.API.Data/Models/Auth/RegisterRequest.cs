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
    [MinLength(8, ErrorMessage = "Het wachtwoord moet minimaal 8 tekens lang zijn.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
        ErrorMessage = "Het wachtwoord moet minimaal één kleine letter, één hoofdletter, één cijfer en één speciaal teken bevatten."
    )]
    public string Password { get; set; } = string.Empty;
}