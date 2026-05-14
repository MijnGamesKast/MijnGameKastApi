using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Catalog;

namespace MijnGameKast.API.Services.Interfaces;

public interface IPublicCollectionService
{
    Task<List<Collection>> GetPublicCollectionsAsync();
    Task<Collection?> GetPublicCollectionByIdAsync(int collectionId);
    Task<List<GameResponse>?> GetPublicCollectionGamesAsync(int collectionId);
}