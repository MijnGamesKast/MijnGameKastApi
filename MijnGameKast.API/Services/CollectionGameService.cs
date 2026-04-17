using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Services;

public class CollectionGameService : ICollectionGameService
{
    private readonly ICollectionGameRepository _collectionGameRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ICatalogRepository _catalogRepository;
    private readonly ISessionRepository _sessionRepository;

    public CollectionGameService(ICollectionGameRepository collectionGameRepository, ICollectionRepository collectionRepository, ICatalogRepository catalogRepository, ISessionRepository sessionRepository)
    {
        _collectionGameRepository = collectionGameRepository;
        _collectionRepository = collectionRepository;
        _catalogRepository = catalogRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<List<Game>?> GetGamesByCollectionIdAsync(int collectionId, string token)
    {
        // Get session and verify
        var session = await _sessionRepository.GetByTokenAsync(token);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }
        
        // Retrieve collection
        var collection = await _collectionRepository.GetByIdAsync(collectionId);

        // Control if there is a collection and for the correct user
        if (collection == null || collection.UserId != session.UserId)
        {
            return null;
        }

        return await _collectionGameRepository.GetGamesByCollectionIdAsync(collectionId);
    }

    public async Task<bool> AddGameToCollectionAsync(int collectionId, int gameId, string token)
    {
        // Retrieve session info and verify it
        var session = await _sessionRepository.GetByTokenAsync(token);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            return false; // Not a valid session
        }
        
        // get collection with collectionId
        var collection = await _collectionRepository.GetByIdAsync(collectionId);
        if (collection == null || collection.UserId != session.UserId)
        {
            return false; // Collection does not exist
        }

        var game = await _catalogRepository.GetByIdAsync(gameId);
        if (game == null)
        {
            return false; // Game does not exist
        }

        var exisitingCollectionGame = await _collectionGameRepository.GetByIdsAsync(collectionId, gameId);
        if (exisitingCollectionGame != null)
        {
            return false; // Game is already in collection 
        }

        var collectionGame = new CollectionGame
        {
            CollectionId = collectionId,
            GameId = gameId,
            CreatedAt = DateTime.UtcNow
        };

        await _collectionGameRepository.AddAsync(collectionGame);

        return true;
    }

    public async Task<bool> RemoveGameFromCollectionAsync(int collectionId, int gameId, string token)
    {
        // Retrieve session info and verify it
        var session = await _sessionRepository.GetByTokenAsync(token);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            return false; // Not a valid session
        }
        
        // get collection with collectionId
        var collection = await _collectionRepository.GetByIdAsync(collectionId);
        if (collection == null || collection.UserId != session.UserId)
        {
            return false; // Collection does not exist
        }
        
        // Get the game from the collection to check if it exist
        var existingCollectionGame = await _collectionGameRepository.GetByIdsAsync(collectionId, gameId);
        if (existingCollectionGame == null)
        {
            return false; // Game does not exist in collection
        }

        return await _collectionGameRepository.DeleteAsync(collectionId, gameId); // Game successfully deleted
    }
}