using System.ComponentModel.DataAnnotations;

namespace MijnGameKast.API.Data.Models;

public class Collection
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Vul een naam voor de collectie in.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Vul een beshchrijving in voor de collectie.")]
    public string Description { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublic { get; set; }
}