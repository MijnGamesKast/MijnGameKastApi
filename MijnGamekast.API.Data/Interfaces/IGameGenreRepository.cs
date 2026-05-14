using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Interfaces;

public interface IGameGenreRepository
{
    Task<List<GameGenre>> GetByGameIdAsync(int gameId);
    Task<List<GameGenre>> GetByGenreIdAsync(int genreId);
    Task<List<Genre>> GetGenreByGameIdAsync(int gameId);
    Task<GameGenre?> GetByIdsAsync(int gameId, int genreId);
    Task<GameGenre> AddAsync(GameGenre gameGenre);
    Task<List<GameGenre>> AddMultipleAsync(List<GameGenre> gameGenres);
    Task<bool> DeleteAsync(int gameId, int genreId);
    Task<int> DeleteByGameIdAsync(int gameId);
}