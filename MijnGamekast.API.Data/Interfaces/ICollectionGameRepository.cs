using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Interfaces;

public interface ICollectionGameRepository
{
    Task<List<CollectionGame>> GetByCollectionIdAsync(int collectionId);
    Task<CollectionGame?> GetByIdsAsync(int collectionId, int gameId);
    Task<CollectionGame> AddAsync(CollectionGame collectionGame);
    Task<bool> DeleteAsync(int collectionId, int gameId);
}