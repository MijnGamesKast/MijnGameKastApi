using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Services.Interfaces;

public interface ICollectionGameService
{
    Task<List<Game>?> GetGamesByCollectionIdAsync(int collectionId, string token);
    Task<bool> AddGameToCollectionAsync(int collectionId, int gameId, string token);
    Task<bool> RemoveGameFromCollectionAsync(int collectionId, int gameId, string token);
}