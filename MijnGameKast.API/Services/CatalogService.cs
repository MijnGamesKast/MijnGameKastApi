using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Catalog;
using MijnGameKast.API.Data.Interfaces;
using MijnGamekast.API.Data.Migrations;
using MijnGameKast.API.Data.Models.Enums;
using MijnGameKast.API.Services.Interfaces;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IGamePlatformRepository _gamePlatformRepository;
    private readonly IGameGenreRepository _gameGenreRepository;
    private readonly ICollectionGameRepository _collectionGameRepository;

    public CatalogService(
        ICatalogRepository catalogRepository,
        IGamePlatformRepository gamePlatformRepository, 
        IGameGenreRepository gameGenreRepository, 
        ICollectionGameRepository collectionGameRepository)
    {
        _catalogRepository = catalogRepository;
        _gamePlatformRepository = gamePlatformRepository;
        _gameGenreRepository = gameGenreRepository;
        _collectionGameRepository = collectionGameRepository;
    }

    public async Task<List<GameResponse>> GetAllAsync()
    {
        var games = await _catalogRepository.GetAllAsync();

        var gameResponses = new List<GameResponse>();

        foreach (var game in games)
        {
            gameResponses.Add(await CreateGameResponse(game));
        }

        return gameResponses;
    }

    public async Task<GameResponse?> GetByIdAsync(int id)
    {
        var game = await _catalogRepository.GetByIdAsync(id);

        if (game == null)
        {
            return null;
        }

        return await CreateGameResponse(game);
    }

    public async Task<GameResponse?> AddGameAsync(CreateGameRequest request, int? userId)
    {
        var game = new Game
        {
            Title = request.Title,
            Description = request.Description,
            UserId = userId,
            Status = GameStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var createdGame = await _catalogRepository.AddGameAsync(game);
        var gameId = createdGame.Id!.Value;
        var gamePlatforms = request.PlatformIds
            .Distinct()
            .Select(platformId => new GamePlatform
            {
                GameId = gameId,
                PlatformId = platformId
            })
            .ToList();

        var gameGenres = request.GenreIds
            .Distinct()
            .Select(genreId => new GameGenre
            {
                GameId = gameId,
                GenreId = genreId
            })
            .ToList();


        if (gamePlatforms.Count > 0)
        {
            await _gamePlatformRepository.AddMultipleAsync(gamePlatforms);
        }

        if (gameGenres.Count > 0)
        {
            await _gameGenreRepository.AddMultipleAsync(gameGenres);
        }

        return await CreateGameResponse(createdGame);
    }

    public async Task<ServiceResult> UpdateGameAsync(int id, UpdateGameRequest request)
    {
        var existingGame = await _catalogRepository.GetByIdAsync(id);

        if (existingGame == null)
        {
            return new ServiceResult
            {
                Message = $"Game met id ({id}) is niet gevonden!",
                Type = ServiceResultType.NotFound
            };
        }
        
        existingGame.Title = request.Title;
        existingGame.Description = request.Description;
        existingGame.Status = request.Status;

        var updated = await _catalogRepository.UpdateGameAsync(existingGame);

        return updated switch
        {
            true => new ServiceResult { Success = true, Message = $"Game met id ({id}) is succesvol bijgewerkt.", Type = ServiceResultType.Success },
            false => new ServiceResult { Message = $"Game met id ({id}) kon niet worden bijgewerkt.", Type = ServiceResultType.BadRequest }
        };
    }

    public async Task<ServiceResult> DeleteGameAsync(int id)
    {
        var existingGame = await _catalogRepository.GetByIdAsync(id);

        if (existingGame == null)
        {
            return new ServiceResult
            {
                Message = $"Game met id ({id}) is niet gevonden!",
                Type = ServiceResultType.NotFound
            };
        }
        
        await _gamePlatformRepository.DeleteByGameIdAsync(id);
        await _gameGenreRepository.DeleteByGameIdAsync(id);
        await _collectionGameRepository.DeleteByGameIdAsync(id);
        
        var deleted = await _catalogRepository.DeleteGameAsync(id);

        return deleted switch
        {
            true => new ServiceResult
            {
                Success = true,
                Message = $"Game met id ({id}) is succesvol verwijderd.",
                Type = ServiceResultType.Success
            },
            false => new ServiceResult
            {
                Message = $"Game met id ({id}) kon niet worden verwijderd.", 
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