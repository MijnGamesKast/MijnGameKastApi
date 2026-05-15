using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Enums;

namespace MijnGameKast.API.Data.Interfaces;

public interface ICatalogRepository
{
    Task<List<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<Game> AddGameAsync(Game game);
    Task<bool> UpdateGameAsync(Game game);
    Task<bool> DeleteGameAsync(int id);
    Task<List<Game>> GetByStatusAsync(GameStatus status);
}