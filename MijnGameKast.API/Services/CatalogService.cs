using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Services.Interfaces;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;

    public CatalogService(ICatalogRepository catalogRepository, ISessionRepository sessionRepository, IUserRepository userRepository)
    {
        _catalogRepository = catalogRepository;
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
    }

    public Task<List<Game>> GetAllAsync()
    {
        return _catalogRepository.GetAllAsync();
    }

    public Task<Game?> GetByIdAsync(int id)
    {
        return _catalogRepository.GetByIdAsync(id);
    }

    public Task<Game> AddGameAsync(Game game)
    {
        return _catalogRepository.AddGameAsync(game);
    }

    public async Task<bool> UpdateGameAsync(int id, Game game)
    {
        game.Id = id;
        return await _catalogRepository.UpdateGameAsync(game);
    }

    public async Task<ServiceResult> DeleteGameAsync(int id, int? userId)
    {
        if (userId == null)
        {
            return new ServiceResult
            {
                Message = "Gebruiker niet gevonden",
                Type = ServiceResultType.BadRequest
            };
        }
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new ServiceResult
            {
                Message = "Gebruiker bestaat niet",
                Type = ServiceResultType.BadRequest
            };
        }

        if (user.Role != UserRole.Moderator)
        {
            return new ServiceResult
            {
                Message = "Gebruiker heeft niet de juiste rechten",
                Type = ServiceResultType.Unauthorized
            };
        }

        var deletedGame = await _catalogRepository.DeleteGameAsync(id);

        if (deletedGame == false)
        {
            // Game does not exist
            return new ServiceResult
            {
                Message = "Spel bestaat niet",
                Type = ServiceResultType.NotFound
            };
        }

        return new ServiceResult
        {
            Success = true,
            Message = $"Game ({id}) is succesvol verwijderd",
            Type = ServiceResultType.Success
        };
    }
}