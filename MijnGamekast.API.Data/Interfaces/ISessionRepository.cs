using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Interfaces;

public interface ISessionRepository
{
    Task<Session> AddAsync(Session session);
    Task<Session?> GetByTokenAsync(string token);
    Task<List<Session>> GetByUserIdAsync(int userId);
    Task<bool> RemoveByTokenAsync(string token);
    Task<bool> RemoveByUserIdAsync(int userId);
    Task<bool> UpdateAsync(Session session);
}