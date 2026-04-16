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
    
    // [Display(Name = "Platform")]
    // public List<string>? Platforms { get; set; }
    //
    // [Display(Name = "Genre")]
    // public List<string>? Genres { get; set; }
}