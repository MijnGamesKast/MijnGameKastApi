using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;

    public GenreService(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }
    
    public async Task<List<Genre>> GetAllAsync()
    {
        return await _genreRepository.GetAllAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _genreRepository.GetByIdAsync(id);
    }

    public async Task<Genre> AddGenre(Genre genre)
    {
        return await _genreRepository.AddGenreAsync(genre);
    }

    public async Task<ServiceResult> UpdateGenreAsync(int id, Genre genre)
    {
        var existingGenre = await _genreRepository.GetByIdAsync(id);

        if (existingGenre == null)
        {
            return new ServiceResult
            {
                Message = $"Genre met id ({id}) is niet gevonden!",
                Type = ServiceResultType.NotFound
            };
        }

        existingGenre.Id = id;
        existingGenre.GenreName= genre.GenreName;
        
        var result = await _genreRepository.UpdateGenreAsync(existingGenre);
        
        return result switch
        {
            true => new ServiceResult { Success = true, Message = $"Genre ({id}) is successvol bijgewerkt", Type = ServiceResultType.Success },
            false => new ServiceResult { Message = "Genre kan niet worden gevonden", Type = ServiceResultType.NotFound }
        };
    }

    public async Task<ServiceResult> DeleteGenreAsync(int id)
    {
        var result = await _genreRepository.DeleteGenreAsync(id);

        return result switch
        {
            true => new ServiceResult { Success = true, Message = $"Genre ({id}) is successvol verwijderd", Type = ServiceResultType.Success },
            false => new ServiceResult { Message = "Genre kan niet worden gevonden", Type = ServiceResultType.NotFound }
        };
    }
}