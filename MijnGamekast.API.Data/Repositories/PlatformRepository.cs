using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Repositories;

public class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext _dbContext;

    public PlatformRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Platform>> GetAllAsync()
    {
        return await _dbContext.Platforms.OrderBy(p => p.Id).ToListAsync();
    }

    public async Task<Platform?> GetByIdAsync(int id)
    {
        return await _dbContext.Platforms.FindAsync(id);
    }

    public async Task<Platform> AddPlatformAsync(Platform platform)
    {
        _dbContext.Platforms.Add(platform);
        await _dbContext.SaveChangesAsync();

        return platform;
    }

    public async Task<bool> UpdatePlatformAsync(Platform platform)
    {
        var existingPlatform = await _dbContext.Platforms.FindAsync(platform.Id);
        if (existingPlatform == null)
        {
            return false;
        }
        
        existingPlatform.PlatformName = platform.PlatformName;
        
        await _dbContext.SaveChangesAsync();
        return true;       
    }

    public async Task<bool> DeletePlatformAsync(int id)
    {
        var platform = await _dbContext.Platforms.FindAsync(id);

        if (platform == null)
        {
            return false;
        }

        _dbContext.Platforms.Remove(platform);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}