using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Services.interfaces;

public interface ICatalogService
{
    Task<List<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<Game> AddGameAsync(Game game);
    Task<bool> UpdateGameAsync(int id, Game game);
    Task<bool> DeleteGameAsync(int id);
}