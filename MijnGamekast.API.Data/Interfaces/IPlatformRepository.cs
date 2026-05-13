using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Interfaces;

public interface IPlatformRepository
{
    Task<List<Platform>> GetAllAsync();
    Task<Platform?> GetByIdAsync(int id);
    Task<Platform> AddPlatformAsync(Platform platform);
    Task<bool> DeletePlatformAsync(int id);
}