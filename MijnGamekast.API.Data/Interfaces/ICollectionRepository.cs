using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Interfaces;

public interface ICollectionRepository
{
    Task<List<Collection>> GetByUserIdAsync(int userId);
    Task<Collection?> GetByIdAsync(int id);
    Task<Collection> AddAsync(Collection collection);
    Task<bool> UpdateAsync(Collection collection);
    Task<bool> DeleteAsync(int id);
}