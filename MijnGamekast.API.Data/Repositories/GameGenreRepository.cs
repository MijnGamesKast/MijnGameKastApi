using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Repositories;

public class GameGenreRepository : IGameGenreRepository
{
    private readonly AppDbContext _dbContext;

    public GameGenreRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<GameGenre>> GetByGameIdAsync(int gameId)
    {
        return await _dbContext.GameGenres.Where(gg => gg.GameId == gameId).ToListAsync();
    }

    public async Task<List<GameGenre>> GetByGenreIdAsync(int genreId)
    {
        return await _dbContext.GameGenres.Where(gg => gg.GenreId == genreId).ToListAsync();
    }

    public async Task<GameGenre?> GetByIdsAsync(int gameId, int genreId)
    {
        return await _dbContext.GameGenres.FirstOrDefaultAsync(gg => gg.GameId == gameId && gg.GenreId == genreId);
    }

    public async Task<GameGenre> AddAsync(GameGenre gameGenre)
    {
        _dbContext.GameGenres.Add(gameGenre);
        await _dbContext.SaveChangesAsync();

        return gameGenre;
    }

    public async Task<List<GameGenre>> AddMultipleAsync(List<GameGenre> gameGenres)
    {
        _dbContext.GameGenres.AddRange(gameGenres);
        await _dbContext.SaveChangesAsync();

        return gameGenres;
    }

    public async Task<bool> DeleteAsync(int gameId, int genreId)
    {
        var gameGenre =
            await _dbContext.GameGenres.FirstOrDefaultAsync(gg => gg.GameId == gameId && gg.GenreId == genreId);

        if (gameGenre == null)
        {
            return false;
        }

        return true;
    }

    public async Task<int> DeleteByGameIdAsync(int gameId)
    {
        var gameGenres = await _dbContext.GameGenres
            .Where(gg => gg.GameId == gameId)
            .ToListAsync();

        if (gameGenres.Count == 0)
        {
            return 0;
        }
        
        _dbContext.GameGenres.RemoveRange(gameGenres);
        await _dbContext.SaveChangesAsync();

        return gameGenres.Count;
    }
}