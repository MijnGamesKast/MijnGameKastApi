using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Repositories;

public class CollectionGameRepository : ICollectionGameRepository
{
    private readonly AppDbContext _dbContext;

    public CollectionGameRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CollectionGame>> GetByCollectionIdAsync(int collectionId)
    {
        return await _dbContext.CollectionGames.Where(c => c.CollectionId == collectionId).ToListAsync();
    }

    public async Task<CollectionGame?> GetByIdsAsync(int collectionId, int gameId)
    {
        return await _dbContext.CollectionGames.FirstOrDefaultAsync(c => c.CollectionId == collectionId && c.GameId == gameId);
    }

    public async Task<CollectionGame> AddAsync(CollectionGame collectionGame)
    {
        _dbContext.CollectionGames.Add(collectionGame);
        await _dbContext.SaveChangesAsync();

        return collectionGame;
    }

    public async Task<bool> DeleteAsync(int collectionId, int gameId)
    {
        var collectionGame =
            await _dbContext.CollectionGames.FirstOrDefaultAsync(c =>
                c.CollectionId == collectionId && c.GameId == gameId);

        if (collectionGame == null)
        {
            return false;
        }

        _dbContext.CollectionGames.Remove(collectionGame);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<Game>> GetGamesByCollectionIdAsync(int collectionId)
    {
        return await _dbContext.CollectionGames
            .Where(cg => cg.CollectionId == collectionId)
            .Join(
                _dbContext.Games,
                cg => cg.GameId,
                g => g.Id,
                (cg, g) => g
            )
            .ToListAsync();
    }
}