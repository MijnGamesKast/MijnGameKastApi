using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services;

public class PlatformService : IPlatformService
{
    private readonly IPlatformRepository _platformRepository;

    public PlatformService(IPlatformRepository platformRepository, ISessionRepository sessionRepository, IUserRepository userRepository)
    {
        _platformRepository = platformRepository;
    }
    
    public async Task<List<Platform>> GetAllAsync()
    {
        return await _platformRepository.GetAllAsync();
    }

    public async Task<Platform?> GetByIdAsync(int id)
    {
        return await _platformRepository.GetByIdAsync(id);
    }

    public async Task<Platform> AddPlatform(Platform platform)
    {
        return await _platformRepository.AddPlatformAsync(platform);
    }

    public async Task<ServiceResult> UpdatePlatformAsync(int id, Platform platform)
    {
        var existingPlatform = await _platformRepository.GetByIdAsync(id);
        if (existingPlatform == null)
        {
            return new ServiceResult
            {
                Message = $"Platform met id ({id}) is niet gevonden!",
                Type = ServiceResultType.NotFound
            };
        }

        existingPlatform.Id = id;
        existingPlatform.PlatformName = platform.PlatformName;
        
        var result = await _platformRepository.UpdatePlatformAsync(existingPlatform);
        
        return result switch
        {
            true => new ServiceResult { Success = true, Message = $"Platform ({id}) is successvol bijgewerkt", Type = ServiceResultType.Success },
            false => new ServiceResult { Message = $"Platform kan niet gevonden geworden", Type = ServiceResultType.NotFound }
        };
    }

    public async Task<ServiceResult> DeletePlatformAsync(int id)
    {
        var result = await _platformRepository.DeletePlatformAsync(id);
        
        return result switch
        {
            true => new ServiceResult { Success = true, Message = $"Platform ({id}) is succesvol verwijderd", Type = ServiceResultType.Success },
            false => new ServiceResult { Message = $"Platform kan niet gevonden geworden", Type = ServiceResultType.NotFound }
        };
    }
}