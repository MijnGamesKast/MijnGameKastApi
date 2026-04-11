using MijnGamekast.API.Data.Interfaces;
using MijnGameKast.API.Models;

namespace MijnGamekast.API.Data.Repositories;

public class GameCatalogRepository : IGameCatalogRepository
{
    private static List<Game> _games = new List<Game>()
    {
        new Game() { Id = 1, Title = "Grand theft auto V", Description = ""},
        new Game() { Id = 2, Title = "Bloodborne", Description = ""}
    };
    
    
}