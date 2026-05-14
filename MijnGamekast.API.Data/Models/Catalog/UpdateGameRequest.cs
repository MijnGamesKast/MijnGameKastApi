using System.ComponentModel.DataAnnotations;
using MijnGameKast.API.Data.Models.Enums;

namespace MijnGameKast.API.Data.Models.Catalog;

public class UpdateGameRequest
{
    [Display(Name = "Titel")]
    [MaxLength(50, ErrorMessage = "De titel mag maximaal {1} tekens bevatten")]
    [Required(ErrorMessage = "Vul de titel in.")]
    public string Title { get; set; } = string.Empty;
    
    [Display(Name = "Beschrijving")]
    [MaxLength(2000, ErrorMessage = "De beschrijving mag maximaal {1} tekens bevatten.")]
    [Required(ErrorMessage = "Vul de beschrijving in.")]
    public string Description { get; set; } = string.Empty;
}