using MijnGameKast.API.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MijnGameKast.API.Data.Models;

public class Game
{
    [Display(Name = "Id")]
    public int? Id { get; set; }
    
    [Display(Name = "Titel")]
    [Required(ErrorMessage = "Vul de titel in.")]
    public string Title { get; set; }
    
    [Display(Name = "Beschrijving")]
    [Required(ErrorMessage = "Vul uw beschrijving in.")]
    public string Description { get; set; } = string.Empty;
    
    [Display(Name = "Gebruiker")]
    public int? UserId { get; set; }
    
    [Display(Name = "Game Status")]
    public GameStatus Status { get; set; } = GameStatus.Pending;
    
    [Display(Name = "Aanmaakdatum")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}