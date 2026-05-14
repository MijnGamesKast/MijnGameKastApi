using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Interfaces;

public interface IGamePlatformRepository
{
    Task<List<GamePlatform>> GetByGameIdAsync(int gameId);
    Task<List<GamePlatform>> GetByPlatformIdAsync(int platformId);
    Task<List<Platform>> GetPlatformByGameIdAsync(int gameId);
    Task<GamePlatform?> GetByIdsAsync(int gameId, int platformId);
    Task<GamePlatform> AddAsync(GamePlatform gamePlatform);
    Task<List<GamePlatform>> AddMultipleAsync(List<GamePlatform> gamePlatforms);
    Task<bool> DeleteAsync(int gameId, int platformId);
    Task<int> DeleteByGameIdAsync(int gameId);
}