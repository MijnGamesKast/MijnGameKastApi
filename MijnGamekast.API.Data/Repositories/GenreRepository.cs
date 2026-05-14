using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly AppDbContext _dbContext;

    public GenreRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Genre>> GetAllAsync()
    {
        return await _dbContext.Genres.OrderBy(g => g.Id).ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _dbContext.Genres.FindAsync(id);
    }

    public async Task<Genre> AddGenreAsync(Genre genre)
    {
        _dbContext.Genres.Add(genre);
        await _dbContext.SaveChangesAsync();

        return genre;
    }

    public async Task<bool> DeleteGenreAsync(int id)
    {
        var genre = await _dbContext.Genres.FindAsync(id);

        if (genre == null)
        {
            return false;
        }

        _dbContext.Genres.Remove(genre);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}