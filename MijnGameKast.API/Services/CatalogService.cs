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

    public async Task<List<Game>> GetAllAsync()
    {
        return await _catalogRepository.GetAllAsync();
    }

    public async Task<Game?> GetByIdAsync(int id)
    {
        return await _catalogRepository.GetByIdAsync(id);
    }

    public async Task<Game> AddGameAsync(Game game)
    {
        return await _catalogRepository.AddGameAsync(game);
    }

    public async Task<ServiceResult> UpdateGameAsync(int gameId, Game game)
    {
        game.Id = gameId;
        var result = await _catalogRepository.UpdateGameAsync(game);
        
        return result switch
        {
            true => new ServiceResult { Success = true, Message = $"game: {game.Title} is aangepast", Type = ServiceResultType.Success },
            false => new ServiceResult { Message = $"er kan geen game gevonden worden met het id: {game.Id}", Type = ServiceResultType.BadRequest },
        };
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