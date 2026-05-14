using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Catalog;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services.Interfaces;

public interface ICollectionGameService
{
    Task<List<GameResponse>> GetGamesByCollectionIdAsync(int collectionId, int? userId);
    Task<ServiceResult> AddGameToCollectionAsync(int collectionId, int gameId, int? userId);
    Task<ServiceResult> RemoveGameFromCollectionAsync(int collectionId, int gameId, int? userId);
}