using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services.Interfaces;

public interface ICatalogService
{
    Task<List<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<Game> AddGameAsync(Game game);
    Task<ServiceResult> UpdateGameAsync(int gameId, Game game);
    Task<ServiceResult> DeleteGameAsync(int id, int? userId);
}