using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<string?> GetUsernameByIdAsync(int id);
    Task<User?> GetByIdAsync(int? id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> AddUserAsync(User user);
    Task<bool> RemoveUserAsync(int id);
}