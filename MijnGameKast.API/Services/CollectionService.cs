using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Services;

public class CollectionService : ICollectionService
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly ISessionRepository _sessionRepository;

    public CollectionService(ICollectionRepository collectionRepository, ISessionRepository sessionRepository)
    {
        _collectionRepository = collectionRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<List<Collection>?> GetMyCollectionsAsync(string token)
    {
        // Get session
        var session = await _sessionRepository.GetByTokenAsync(token);

        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }
        
        return await _collectionRepository.GetByUserIdAsync(session.UserId);
    }

    public async Task<Collection?> GetByIdAsync(int id, string token)
    {
        // Get session and check if it is valid
        var session = await _sessionRepository.GetByTokenAsync(token);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        // Get collection and check if it exists
        var collection = await _collectionRepository.GetByIdAsync(id);
        if (collection == null)
        {
            return null;
        }
        
        // Check if the collection belongs to the user
        if (collection.UserId != session.UserId)
        {
            return null;
        }

        return collection;
    }

    public async Task<Collection?> CreateAsync(Collection collection, string token)
    {
        var session = await _sessionRepository.GetByTokenAsync(token);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }
        
        collection.UserId = session.UserId;
        collection.CreatedAt = DateTime.UtcNow;

        return await _collectionRepository.AddAsync(collection);
    }

    public async Task<bool> UpdateAsync(int id, Collection collection, string token)
    {
        var session = await _sessionRepository.GetByTokenAsync(token);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            return false;
        }
        
        var existingCollection = await _collectionRepository.GetByIdAsync(id);

        if (existingCollection == null || existingCollection.UserId != session.UserId)
        {
            return false;
        }

        collection.Id = id;
        collection.UserId = existingCollection.UserId;
        collection.CreatedAt = existingCollection.CreatedAt;
        
        return await _collectionRepository.UpdateAsync(collection);
    }

    public async Task<bool> DeleteAsync(int id, string token)
    {
        var session = await _sessionRepository.GetByTokenAsync(token);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            return false;
        }

        var existingCollection = await _collectionRepository.GetByIdAsync(id);

        if (existingCollection == null)
        {
            return false;
        }

        if (existingCollection.UserId != session.UserId)
        {
            return false;
        }

        return await _collectionRepository.DeleteAsync(id);
    }
}