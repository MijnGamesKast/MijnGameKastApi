using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Services;

public class CollectionService : ICollectionService
{
    private readonly ICollectionRepository _collectionRepository;

    public CollectionService(ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }

    public async Task<List<Collection>?> GetMyCollectionsAsync(int? userId)
    {
        if (userId == null)
        {
            return null;
        }
        
        return await _collectionRepository.GetByUserIdAsync(userId);
    }

    public async Task<Collection?> GetByIdAsync(int id, int? userId)
    {
        // Get collection and check if it exists
        var collection = await _collectionRepository.GetByIdAsync(id);
        if (collection == null)
        {
            return null;
        }
        
        // Check if the collection belongs to the user
        if (collection.UserId != userId)
        {
            return null;
        }

        return collection;
    }

    public async Task<Collection?> CreateAsync(Collection collection, int? userId)
    {
        collection.UserId = userId ?? 0;
        collection.CreatedAt = DateTime.UtcNow;

        return await _collectionRepository.AddAsync(collection);
    }

    public async Task<bool> UpdateAsync(int id, Collection collection, int? userId)
    {
        var existingCollection = await _collectionRepository.GetByIdAsync(id);

        if (existingCollection == null || existingCollection.UserId != userId)
        {
            return false;
        }

        collection.Id = id;
        collection.UserId = existingCollection.UserId;
        collection.CreatedAt = existingCollection.CreatedAt;
        
        return await _collectionRepository.UpdateAsync(collection);
    }

    public async Task<bool> DeleteAsync(int id, int? userId)
    {
        var existingCollection = await _collectionRepository.GetByIdAsync(id);

        if (existingCollection == null)
        {
            return false;
        }

        if (existingCollection.UserId != userId)
        {
            return false;
        }

        return await _collectionRepository.DeleteAsync(id);
    }
}