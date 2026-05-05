using System.ComponentModel.DataAnnotations;

namespace MijnGameKast.API.Data.Models;

public class User
{
    public int Id { get; set; }
    
    [Display(Name = "Gebruikersnaam")]
    [Required(ErrorMessage = "Vul uw {0} in.")]
    public string Username { get; set; }  = string.Empty;
    
    [Display(Name = "E-mailadres")]
    [Required(ErrorMessage = "Vul uw {0} in.")]
    [EmailAddress(ErrorMessage = "Vul een geldig e-mailadres in.")]
    public string Email { get; set; }  = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; }  = string.Empty;
    
    public DateTime CreatedAt { get; set; }

    public UserRole Role { get; set; } = UserRole.Gamer;
}