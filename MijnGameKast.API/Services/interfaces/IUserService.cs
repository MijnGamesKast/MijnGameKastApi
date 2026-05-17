using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<string?> GetUsernameByIdAsync(int id);
}