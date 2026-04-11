using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace MijnGameKast.API.Models;

public class Game
{
    [Display(Name = "Id")]
    [Required(ErrorMessage = "Vul uw ID in.")]
    public int Id { get; set; }
    
    [Display(Name = "Titel")]
    [Required(ErrorMessage = "Vul de titel in.")]
    public string Title { get; set; } = string.Empty;
    
    [Display(Name = "Beschrijving")]
    [Required(ErrorMessage = "Vul uw beschrijving in.")]
    public string Description { get; set; } = string.Empty;
    public List<string>? Platforms { get; set; }
    public List<string>? Genres { get; set; }
}