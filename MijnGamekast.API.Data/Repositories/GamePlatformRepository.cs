using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Repositories;

public class GamePlatformRepository : IGamePlatformRepository
{
    private readonly AppDbContext _dbContext;

    public GamePlatformRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<GamePlatform>> GetByGameIdAsync(int gameId)
    {
        return await _dbContext.GamePlatforms.Where(gp => gp.GameId == gameId).ToListAsync();
    }

    public async Task<List<GamePlatform>> GetByPlatformIdAsync(int platformId)
    {
        return await _dbContext.GamePlatforms.Where(gp => gp.PlatformId == platformId).ToListAsync();
    }

    public async Task<List<Platform>> GetPlatformByGameIdAsync(int gameId)
    {
        return await _dbContext.GamePlatforms
            .Where(gp => gp.GameId == gameId)
            .Join(
                _dbContext.Platforms,
                gp => gp.PlatformId,
                p => p.Id,
                (gp, p) => p)
            .ToListAsync();
    }

    public async Task<GamePlatform?> GetByIdsAsync(int gameId, int platformId)
    {
        return await _dbContext.GamePlatforms.FirstOrDefaultAsync(gp =>
            gp.GameId == gameId && gp.PlatformId == platformId);
    }

    public async Task<GamePlatform> AddAsync(GamePlatform gamePlatform)
    {
        _dbContext.GamePlatforms.Add(gamePlatform);
        await _dbContext.SaveChangesAsync();

        return gamePlatform;
    }

    public async Task<List<GamePlatform>> AddMultipleAsync(List<GamePlatform> gamePlatforms)
    {
        _dbContext.GamePlatforms.AddRange(gamePlatforms);
        await _dbContext.SaveChangesAsync();

        return gamePlatforms;
    }

    public async Task<bool> DeleteAsync(int gameId, int platformId)
    {
        var gamePlatform =
            await _dbContext.GamePlatforms.FirstOrDefaultAsync(gp =>
                gp.GameId == gameId && gp.PlatformId == platformId);

        if (gamePlatform == null)
        {
            return false;
        }

        return true;
    }

    public async Task<int> DeleteByGameIdAsync(int gameId)
    {
        var gamePlatforms = await _dbContext.GamePlatforms
            .Where(gp => gp.GameId == gameId)
            .ToListAsync();

        if (gamePlatforms.Count == 0)
        {
            return 0;
        }

        _dbContext.GamePlatforms.RemoveRange(gamePlatforms);
        await _dbContext.SaveChangesAsync();

        return gamePlatforms.Count;
    }
}