using MijnGameKast.API.Data.Models.Enums;

namespace MijnGameKast.API.Data.Models.Catalog;

public class GameResponse
{
    public int? Id { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int? UserId { get; set; }
    
    public string status { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public List<Platform> Platforms { get; set; } = new();
    
    public List<Genre> Genres { get; set; } = new();
}