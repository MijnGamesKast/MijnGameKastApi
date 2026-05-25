using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services.Interfaces;

public interface IGenreService
{
    Task<List<Genre>> GetAllAsync();
    Task<Genre?> GetByIdAsync(int id);
    Task<Genre> AddGenre(Genre genre);
    Task<ServiceResult> UpdateGenreAsync(int id, Genre genre);
    Task<ServiceResult> DeleteGenreAsync(int id);
}