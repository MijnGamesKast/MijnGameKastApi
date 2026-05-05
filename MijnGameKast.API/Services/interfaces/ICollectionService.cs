using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Services.Interfaces;

public interface ICollectionService
{
    Task<List<Collection>?> GetMyCollectionsAsync(int? userId);
    Task<Collection?> GetByIdAsync(int id, int? userId);
    Task<Collection?> CreateAsync(Collection collection, int? userId);
    Task<bool> UpdateAsync(int id, Collection collection, int? userId);
    Task<bool> DeleteAsync(int id, int? userId);
}