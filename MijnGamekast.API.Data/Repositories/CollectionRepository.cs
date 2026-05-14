using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Repositories;

public class CollectionRepository : ICollectionRepository
{
    private readonly AppDbContext _dbContext;
    
    public CollectionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Collection>> GetByUserIdAsync(int? userId)
    {
        return await _dbContext.Collections
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Id)
            .ToListAsync();   
    }

    public async Task<Collection?> GetByIdAsync(int id)
    {
        return await _dbContext.Collections.FindAsync(id);
    }

    public async Task<Collection> AddAsync(Collection collection)
    {
        _dbContext.Collections.Add(collection);
        await _dbContext.SaveChangesAsync();

        return collection;
    }

    public async Task<bool> UpdateAsync(Collection collection)
    {
        var existingCollection = await _dbContext.Collections.FindAsync(collection.Id);
        if (existingCollection == null)
        {
            return false;
        }
        
        existingCollection.Name = collection.Name;
        existingCollection.Description = collection.Description;
        existingCollection.IsPublic = collection.IsPublic;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var collection = await _dbContext.Collections.FindAsync(id);
        if (collection == null)
        {
            return false;
        }

        _dbContext.Collections.Remove(collection);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<Collection>> GetPublicCollectionsAsync()
    {
        return await _dbContext.Collections
            .Where(c => c.IsPublic)
            .OrderBy(c => c.Id)
            .ToListAsync();  
    }
}