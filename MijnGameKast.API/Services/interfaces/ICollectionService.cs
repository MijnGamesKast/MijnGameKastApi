using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Services.Interfaces;

public interface ICollectionService
{
    Task<List<Collection>?> GetMyCollectionsAsync(string token);
    Task<Collection?> GetByIdAsync(int id, string token);
    Task<Collection?> CreateAsync(Collection collection, string token);
    Task<bool> UpdateAsync(int id, Collection collection, string token);
    Task<bool> DeleteAsync(int id, string token);
}