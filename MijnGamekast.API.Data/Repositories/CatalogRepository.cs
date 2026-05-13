using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MijnGameKast.API.Data.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly AppDbContext _dbContext;

    public CatalogRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Game>> GetAllAsync()
    {
        return await _dbContext.Games.OrderBy(g => g.Id).ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(int id)
    {
        return await _dbContext.Games.FindAsync(id);
    }
    
    public async Task<Game> AddGameAsync(Game game)
    {
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        return game;
    }

    public async Task<bool> UpdateGameAsync(Game game)
    {
        var existingGame = await _dbContext.Games.FindAsync(game.Id);

        if (existingGame == null)
        {
            return false;
        }

        existingGame.Title = game.Title;
        existingGame.Description = game.Description;
        
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteGameAsync(int id)
    {
        var game = await _dbContext.Games.FindAsync(id);

        if (game == null)
        {
            return false;
        }

        _dbContext.Games.Remove(game);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}