using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Interfaces;

public interface IGenreRepository
{
    Task<List<Genre>> GetAllAsync();
    Task<Genre?> GetByIdAsync(int id);
    Task<Genre> AddGenreAsync(Genre genre);
    Task<bool> UpdateGenreAsync(Genre genre);
    Task<bool> DeleteGenreAsync(int id);
}