using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Catalog;
using MijnGameKast.API.Services.Interfaces;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services;

public class CollectionGameService : ICollectionGameService
{
    private readonly ICollectionGameRepository _collectionGameRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ICatalogRepository _catalogRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IGamePlatformRepository _gamePlatformRepository;
    private readonly IGameGenreRepository _gameGenreRepository;
    
    public CollectionGameService(
        ICollectionGameRepository collectionGameRepository, 
        ICollectionRepository collectionRepository, 
        ICatalogRepository catalogRepository,
        ISessionRepository sessionRepository,
        IGamePlatformRepository gamePlatformRepository,
        IGameGenreRepository gameGenreRepository
        )
    {
        _collectionGameRepository = collectionGameRepository;
        _collectionRepository = collectionRepository;
        _catalogRepository = catalogRepository;
        _sessionRepository = sessionRepository;
        _gamePlatformRepository = gamePlatformRepository;
        _gameGenreRepository = gameGenreRepository;
    }

    public async Task<List<GameResponse>> GetGamesByCollectionIdAsync(int collectionId, int? userId)
    {
        // Retrieve collection
        var collection = await _collectionRepository.GetByIdAsync(collectionId);

        // Control if there is a collection and for the correct user
        if (collection == null || collection.UserId != userId)
        {
            return null;
        }
        
        var collectionGame = await _collectionGameRepository.GetGamesByCollectionIdAsync(collectionId);
        
        var gameResponse = new List<GameResponse>();

        foreach (var game in collectionGame)
        {
            gameResponse.Add(await CreateGameResponse(game));
        }

        return gameResponse;
    }

    public async Task<ServiceResult> AddGameToCollectionAsync(int collectionId, int gameId, int? userId)
    {
        // get collection with collectionId
        var collection = await _collectionRepository.GetByIdAsync(collectionId);
        if (collection == null || collection.UserId != userId)
        {
            return new ServiceResult
            {
                Message = $"Collection met id ({collectionId}) kan niet gevonden worden",
                Type = ServiceResultType.NotFound
            };
        }

        var game = await _catalogRepository.GetByIdAsync(gameId);
        if (game == null)
        {
            return new ServiceResult
            {
                Message = $"Game met id ({gameId}) is niet gevonden",
                Type = ServiceResultType.NotFound
            };
        }

        var exisitingCollectionGame = await _collectionGameRepository.GetByIdsAsync(collectionId, gameId);
        if (exisitingCollectionGame != null)
        {
            return new ServiceResult
            {
                Message = $"Game met id ({gameId}) is al in de collectie ({collectionId})",
                Type = ServiceResultType.Conflict
            };
        }

        var collectionGame = new CollectionGame
        {
            CollectionId = collectionId,
            GameId = gameId,
            CreatedAt = DateTime.UtcNow
        };

        await _collectionGameRepository.AddAsync(collectionGame);

        return new ServiceResult
        {
            Success = true,
            Message = $"Game met id ({gameId}) is succesvol toegevoegd aan de collectie ({collectionId})",
            Type = ServiceResultType.Success
        };
    }

    public async Task<ServiceResult> RemoveGameFromCollectionAsync(int collectionId, int gameId, int? userId)
    {
        // get collection with collectionId
        var collection = await _collectionRepository.GetByIdAsync(collectionId);
        if (collection == null || collection.UserId != userId)
        {
            return new ServiceResult
            {
                Message = $"Collection met id ({collectionId}) kan niet gevonden worden",
                Type = ServiceResultType.NotFound
            };
        }
        
        // Get the game from the collection to check if it exist
        var existingCollectionGame = await _collectionGameRepository.GetByIdsAsync(collectionId, gameId);
        if (existingCollectionGame == null)
        {
            return new ServiceResult
            {
                Message = $"Game met id ({gameId}) is niet in de collectie ({collectionId})",
                Type = ServiceResultType.NotFound
            };
        }

        var result = await _collectionGameRepository.DeleteAsync(collectionId, gameId); // Game successfully deleted

        return result switch
        {
            true => new ServiceResult
            {
                Success = true,
                Message = $"Game met id ({gameId}) is succesvol verwijderd uit de collectie ({collectionId})",
                Type = ServiceResultType.Success
            },
            false => new ServiceResult
            {
                Message = $"Game met id ({gameId}) kon niet worden verwijderd uit de collectie ({collectionId})",
                Type = ServiceResultType.BadRequest
            }
        };
    }
    
    private async Task<GameResponse> CreateGameResponse(Game game)
    {
        var gameId = game.Id!.Value;
        
        var platforms = await _gamePlatformRepository.GetPlatformByGameIdAsync(gameId);
        var genres = await _gameGenreRepository.GetGenreByGameIdAsync(gameId);

        return new GameResponse
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            UserId = game.UserId,
            Status = game.Status.ToString(),
            CreatedAt = game.CreatedAt,
            Platforms = platforms,
            Genres = genres
        };
    }
}