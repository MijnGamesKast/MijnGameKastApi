using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Catalog;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Services;

public class PublicCollectionService : IPublicCollectionService
{
    private readonly IGamePlatformRepository _gamePlatformRepository;
    private readonly IGameGenreRepository _gameGenreRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ICollectionGameRepository _collectionGameRepository;

    public PublicCollectionService(
        IGamePlatformRepository gamePlatformRepository,
        IGameGenreRepository gameGenreRepository,
        ICollectionRepository collectionRepository,
        ICollectionGameRepository collectionGameRepository
    )
    {
        _gamePlatformRepository = gamePlatformRepository;
        _gameGenreRepository = gameGenreRepository;
        _collectionRepository = collectionRepository;
        _collectionGameRepository = collectionGameRepository;
    }

    public async Task<List<Collection>> GetPublicCollectionsAsync()
    {
        return await _collectionRepository.GetPublicCollectionsAsync();
    }

    public async Task<Collection?> GetPublicCollectionByIdAsync(int collectionId)
    {
        var collection = await _collectionRepository.GetByIdAsync(collectionId);

        if (collection == null || collection.IsPublic == false)
        {
            return null;
        }

        return collection;
    }

    public async Task<List<GameResponse>?> GetPublicCollectionGamesAsync(int collectionId)
    {
        var collection = await _collectionRepository.GetByIdAsync(collectionId);

        if (collection == null || collection.IsPublic == false)
        {
            return null; // Collection not found or not public
        }

        var games = await _collectionGameRepository.GetGamesByCollectionIdAsync(collectionId);

        var gameResponses = new List<GameResponse>();

        foreach (var game in games)
        {
            gameResponses.Add(await CreateGameResponse(game));
        }

        return gameResponses;
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