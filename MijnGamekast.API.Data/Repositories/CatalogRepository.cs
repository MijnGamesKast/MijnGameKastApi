using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private static List<Game> _games = new List<Game>()
    {
        new Game() { Id = 1, Title = "Grand theft auto V", Description = ""},
        new Game() { Id = 2, Title = "Bloodborne", Description = ""},
        new Game() { Id = 3, Title = "Harry Potter 2", Description = ""},
        new Game() { Id = 4, Title = "The Last of Us", Description = "The Last of Us is a third-person action-adventure game featuring a mix of exploration, stealth and combat. Players face both infected creatures and hostile human enemies while progressing through varied environments. The game includes a narrative-driven single-player campaign and a competitive online multiplayer mode called Factions. Trophy support is included, and additional downloadable content was made available separately."}
    };

    private static int _nextId = 5;


    public Task<List<Game>> GetAllAsync()
    {
        return Task.FromResult(_games.ToList());
    }

    public Task<Game?> GetByIdAsync(int id)
    {
        Game? game = _games.FirstOrDefault(g => g.Id == id);
        return Task.FromResult(game);
    }

    public Task<Game> AddGameAsync(Game game)
    {
        game.Id = _nextId++;
        _games.Add(game);

        return Task.FromResult(game);
    }

    public Task<bool> UpdateGameAsync(Game game)
    {
        var existingGame = _games.FirstOrDefault(g => g.Id == game.Id);
        if (existingGame == null)
        {
            return Task.FromResult(false);
        }
        existingGame.Title = game.Title;
        existingGame.Description = game.Description;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteGameAsync(int id)
    {
        var game = _games.FirstOrDefault(g => g.Id == id);
        if (game == null)
        {
            return Task.FromResult(false);
        }
        _games.Remove(game);
        return Task.FromResult(true);
    }
}