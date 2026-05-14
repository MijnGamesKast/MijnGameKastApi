using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services.Interfaces;

public interface IPlatformService
{
    Task<List<Platform>> GetAllAsync();
    Task<Platform?> GetByIdAsync(int id);
    Task<Platform> AddPlatform(Platform platform);
    Task<ServiceResult> DeletePlatformAsync(int id);
}